using System.Text;

var input = File.ReadAllText("input.txt")
  .Select(x => x.ToString())
  .Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'))
  .Aggregate((s, s1) => s + s1);
  

var reader = new Reader(input);

long versionSum = 0;
var result2 = EvaluatePackage(reader);

Console.Out.WriteLine(versionSum);
Console.Out.WriteLine(result2);

long EvaluatePackage(Reader r) {
  var (version, typeId) = ReadHeader(reader);
  versionSum += version;
  
  switch (typeId) {
    case 0: //sum
      return ReadOperator(reader).Sum();
    case 1: //product
      return ReadOperator(reader).Aggregate((l, l1) => l * l1);
    case 2: //min
      return ReadOperator(reader).Min();
    case 3: //max
      return ReadOperator(reader).Max();
    case 4: //literal
      return ReadLiteral(r);
    case 5: //greater than
      var gt = ReadOperator(r).ToList();
      return gt[0] > gt[1] ? 1 : 0;
    case 6: //less than
      var lt = ReadOperator(r).ToList();
      return lt[0] < lt[1] ? 1 : 0;
    case 7: //eqal to
      var eq = ReadOperator(r).ToList();
      return eq[0] == eq[1] ? 1 : 0;
    default:
      throw new Exception("ohoh");
  }
}

IEnumerable<long> ReadOperator(Reader r) {
  var lengthTypeId = r.Read();
  switch (lengthTypeId) {
    case "0":
      var lenInBits = Convert.ToInt64(r.Read(15), 2);
      var currentPos = r.Position;
      while (r.Position < currentPos + lenInBits) {
        yield return EvaluatePackage(r);
      }
      break;
    case "1":
      var counter = Convert.ToInt64(r.Read(11), 2);
      for (var i = 0; i < counter; i++) {
        yield return EvaluatePackage(r);
      }
      break;
  }
}

(long, long) ReadHeader(Reader r) {
  var v = Convert.ToInt32(r.Read(3), 2);
  var t = Convert.ToInt32(r.Read(3), 2);
  return (v, t);
}

long ReadLiteral(Reader r) {
  SkipZeros(r);
  var number = "";
  var finished = false;
  while (!finished) {
    var isLast = r.Read() == "0";
    number += r.Read(4);    
    finished = isLast;
  }
  return Convert.ToInt64(number, 2);
}

void SkipZeros(Reader r) {
  while (r.Peek() == "0") {
    r.Read();
  }
}

internal class Reader {
  private readonly string _input;

  public Reader(string input) {
    _input = input;
    Position = 0;
  }

  public int Position { get; private set; }

  public string Read(int count = 1) {
    var value = _input.Substring(Position, count);
    Position += count;
    return value;
  }

  public string Peek(int count = 0) {
    return _input.Substring(Position, count);
  }
}