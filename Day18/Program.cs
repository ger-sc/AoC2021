var input = File.ReadAllLines("input.txt").ToList();

var part1 = Part1(input);
var magnitude = CalcMagnitude(part1);
Console.Out.WriteLine(magnitude);

var part2 = Part2(input);
Console.Out.WriteLine(part2);

HierarchyTuple Part1(IReadOnlyCollection<string> lines) {
  var lookup = new List<HierarchyTuple>();
  var a = ParseNumber(input[0].GetEnumerator(), ref lookup);
  var b = ParseNumber(input[1].GetEnumerator(), ref lookup);
  Add(a, b, ref lookup);
  var count = 1;
  while (count < lines.Count) {
    Reduce(ref lookup);
    count++;
    if (count >= input.Count) break;
    var root = lookup.Single(x => x.Parent == null);
    var newItem = ParseNumber(input[count].GetEnumerator(), ref lookup);
    Add(root, newItem, ref lookup);
  }
  return lookup.Single(x => x.Parent == null);
}

long Part2(IList<string> lines) {
  var max = 0L;

  for (var i = 0; i < lines.Count; i++) {
    for (var j = 0; j < lines.Count; j++) {
      if (i == j) continue;
      var lookup2 = new List<HierarchyTuple>();
      var a = ParseNumber(lines[i].GetEnumerator(), ref lookup2);
      var b = ParseNumber(lines[j].GetEnumerator(), ref lookup2);
      Add(a, b, ref lookup2);
      Reduce(ref lookup2);
      var mag2 = CalcMagnitude(lookup2.Single(x => x.Parent == null));
      if (mag2 > max) {
        max = mag2;
      }
    }
  }

  return max;
}

void Reduce(ref List<HierarchyTuple> list) {
  var doneSomething = true;
  while (doneSomething) {
    doneSomething = false;
    var findToExplode = list
      .OrderBy(x => x.Value)
      .FirstOrDefault(x => x.Level > 4 && x.Left.HasValue && x.Right.HasValue);

    if (findToExplode != null) {
      Explode(findToExplode, ref list);
      doneSomething = true;
    }

    if (!doneSomething) {
      var findToSplit = list
        .OrderBy(x => x.Value)
        .FirstOrDefault(x => x.Left is > 9 || x.Right is > 9);
      if (findToSplit != null) {
        Split(findToSplit, ref list);
        doneSomething = true;
      }
    }
  }
}

long CalcMagnitude(HierarchyTuple root) {
  var left = root.Left ?? CalcMagnitude(root.LeftChild);
  var right = root.Right ?? CalcMagnitude(root.RightChild);
  return left * 3 + right * 2;
}

HierarchyTuple ParseNumber(CharEnumerator number, ref List<HierarchyTuple> list, HierarchyTuple? root = null) {
  var tuple = new HierarchyTuple();
  if (root != null) {
    tuple.Parent = root;
  }
  else {
    number.MoveNext();
  }
  number.MoveNext();

  if (number.Current == '[') {
    tuple.LeftChild = ParseNumber(number, ref list, tuple);
  } else if (int.TryParse(number.Current.ToString(), out var leftValue)) {
    tuple.Left = leftValue;
  }
  number.MoveNext();
  if (number.Current == ',') number.MoveNext();
  if (number.Current == '[') {
    tuple.RightChild = ParseNumber(number, ref list, tuple);
  } else if (int.TryParse(number.Current.ToString(), out var rightValue)) {
    tuple.Right = rightValue;
  }
  number.MoveNext();
  if (number.Current == ']') {
    list.Add(tuple);
    return tuple;
  }

  throw new Exception("something went wrong");
}

void Add(HierarchyTuple a, HierarchyTuple b, ref List<HierarchyTuple> list) {
  var tuple = new HierarchyTuple {
    LeftChild = a,
    RightChild = b
  };
  a.Parent = tuple;
  b.Parent = tuple;
  list.Add(tuple);
}

void Explode(HierarchyTuple tuple, ref List<HierarchyTuple> list) {
  if (tuple.Left == null || tuple.Right == null || tuple.Parent == null) 
    throw new ArgumentException("Could not explode tuple");
  
  list.Remove(tuple);
  var leftValue = tuple.Left.Value;
  var rightValue = tuple.Right.Value;
  var findLeft = list
    .Where(x => x.Value < tuple.Value && (x.Left != null || x.Right != null))
    .OrderBy(x => x.Value)
    .LastOrDefault();
  var findRight = list
    .Where(x => x.Value > tuple.Value && (x.Left != null || x.Right != null))
    .OrderBy(x => x.Value)
    .FirstOrDefault();

  if (findLeft != null) {
    if (findLeft.Right != null) {
      findLeft.Right += leftValue;
    }
    else {
      findLeft.Left += leftValue;
    }
  }

  if (findRight != null) {
    if (findRight.Left != null) {
      findRight.Left += rightValue;
    }
    else {
      findRight.Right += rightValue;
    }
  }

  var isLeft = tuple.Parent.LeftChild == tuple;

  if (isLeft) {
    tuple.Parent.LeftChild = null;
    tuple.Parent.Left = 0;
  }
  else {
    tuple.Parent.RightChild = null;
    tuple.Parent.Right = 0;
  }
}

void Split(HierarchyTuple tuple, ref List<HierarchyTuple> list) {
  if (tuple.Left is > 9) {
    var newLeft = (int)Math.Floor(tuple.Left.Value / 2d);
    var newRight = (int)Math.Ceiling(tuple.Left.Value / 2d);
    var newTuple = new HierarchyTuple {
      Parent = tuple,
      Left = newLeft,
      Right = newRight
    };
    tuple.LeftChild = newTuple;
    tuple.Left = null;
    list.Add(newTuple);
  } else if (tuple.Right is > 9) {
    var newLeft = (int)Math.Floor(tuple.Right.Value / 2d);
    var newRight = (int)Math.Ceiling(tuple.Right.Value / 2d);
    var newTuple = new HierarchyTuple {
      Parent = tuple,
      Left = newLeft,
      Right = newRight
    };
    tuple.RightChild = newTuple;
    tuple.Right = null;
    list.Add(newTuple);
  }
  else {
    throw new ArgumentException("Tuple cannot be split");
  }
}

internal class HierarchyTuple {
  public HierarchyTuple? Parent;
  public HierarchyTuple? LeftChild;
  public HierarchyTuple? RightChild;
  public int? Left;
  public int? Right;
  
  public int Level {
    get {
      var level = 1;
      var current = this;
      while (current.Parent != null) {
        current = current.Parent;
        level++;
      }
      return level;
    }
  }

  public float Value {
    get {
      var diff = 0f;
      var level = Level;
      var current = this;
      while (current.Parent != null) {
        var isLeft = current.Parent.LeftChild != null && current.Parent.LeftChild.Equals(current);
        if (isLeft) {
          diff -= 1f / (float)Math.Pow(2, level-2);
        }
        else {
          diff += 1f / (float)Math.Pow(2, level-2);
        }
        current = current.Parent;
        level--;
      }
      return diff;
    }
  }
}