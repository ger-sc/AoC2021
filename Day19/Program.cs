using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

var scannerMaps = ReadInput(input);

CreateNeighbors(scannerMaps);

var scannerLocationDiffs = new Dictionary<(int, int), Point3D>();
var correctRotation = new HashSet<int> { 0 };


while (!MappingComplete(scannerLocationDiffs, scannerMaps.Count)) {
  foreach (var scanner in scannerMaps.Keys) {
    foreach (var innerScanner in scannerMaps.Keys.Where(x => x != scanner)) {
      if (scannerLocationDiffs.ContainsKey((scanner, innerScanner))) continue;
      foreach (var p1 in scannerMaps[scanner]) {
        var found = false;
        var rotation = new List<Func<Point3D, Point3D>>();
        foreach (var p2 in scannerMaps[innerScanner]) {
          if (scannerLocationDiffs.ContainsKey((scanner, innerScanner))) continue;
          var count = p1.Neighbors
            .Concat(p2.Neighbors)
            .GroupBy(x => x.Value)
            .Count(x => x.Count() > 1);

          if (count >= 11 && correctRotation.Contains(scanner)) {
            found = true;
            var (key1, value1) = p1.Neighbors.First(p => p2.Neighbors.Values.Contains(p.Value));
            var (key2, _) = p2.Neighbors.First(p => p.Value == value1);
            var vec1 = Subtract(p1, key1);
            var vec2 = Subtract(p2, key2);
            rotation = GetRotation(vec1, vec2).ToList();

            var pRot = rotation.Aggregate(p2, (current, func) => func(current));

            scannerLocationDiffs[(scanner, innerScanner)] = Subtract(p1, pRot);

            break;
          }
        }

        if (!found) continue;
        if (correctRotation.Contains(innerScanner)) continue;
        scannerMaps[innerScanner] = scannerMaps[innerScanner]
          .Select(point3D => rotation.Aggregate(point3D, (current, func) => func(current))).ToList();
        CreateNeighborList(scannerMaps[innerScanner]);
        correctRotation.Add(innerScanner);
      }
    }
  }
}

var scannerLocations = GetScannerLocations(scannerLocationDiffs);

var correctedMap = CorrectLocation(scannerMaps, scannerLocations);

Console.Out.WriteLine(correctedMap.SelectMany(x => x.Value).Distinct().Count());

var distances = GetMaxDistance(scannerLocations);

Console.Out.WriteLine(distances);

int GetMaxDistance(Dictionary<int, Point3D> scanners) {
  var d = new List<int>();
  foreach (var (_, value) in scanners) {
    d.AddRange(scanners.Select(s2 =>
      Math.Abs(value.X - s2.Value.X) + Math.Abs(value.Y - s2.Value.Y) + Math.Abs(value.Z - s2.Value.Z)));
  }

  return d.Max();
}

Dictionary<int, IList<Point3D>> CorrectLocation(Dictionary<int, IList<Point3D>> map,
  IReadOnlyDictionary<int, Point3D> sensors) {
  var newMap = new Dictionary<int, IList<Point3D>>();
  foreach (var (key, value) in map) {
    newMap[key] = value.Select(p => Add(p, sensors[key])).ToList();
  }

  return newMap;
}

Dictionary<int, Point3D> GetScannerLocations(Dictionary<(int, int), Point3D> locationDiffs) {
  var locations = new Dictionary<int, Point3D> { { 0, new Point3D(0, 0, 0) } };
  var count = locationDiffs.Keys.Select(x => x.Item1).Concat(locationDiffs.Keys.Select(x => x.Item2)).Distinct()
    .Count();
  while (locations.Count < count) {
    foreach (var known in locations.Keys.ToList()) {
      var next = locationDiffs.Where(x => x.Key.Item1 == known);
      foreach (var ((_, item2), value) in next) {
        if (locations.ContainsKey(item2)) continue;
        locations[item2] = Add(locations[known], value);
      }
    }
  }

  return locations;
}

bool MappingComplete(Dictionary<(int, int), Point3D> map, int count) {
  var visited = new HashSet<int> { 0 };
  var change = true;
  while (change) {
    change = false;
    var next = map.Where(x => visited.Contains(x.Key.Item1));
    foreach (var ((_, item2), _) in next) {
      if (visited.Contains(item2)) continue;
      change = true;
      visited.Add(item2);
    }
  }

  return visited.Count == count;
}

IList<Func<Point3D, Point3D>> GetRotation(Point3D vec1, Point3D vec2) {
  if (vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z) return new List<Func<Point3D, Point3D>>();
  var rotResult = new List<Func<Point3D, Point3D>>();

  while (Math.Abs(vec1.X) != Math.Abs(vec2.X) || Math.Abs(vec1.Y) != Math.Abs(vec2.Y) ||
         Math.Abs(vec1.Z) != Math.Abs(vec2.Z)) {
    if (Math.Abs(vec1.X) == Math.Abs(vec2.Y)) {
      vec2 = vec2.RotateZ();
      rotResult.Add(MatrixRotation.RotateZ);
    }
    else if (Math.Abs(vec1.X) == Math.Abs(vec2.Z)) {
      vec2 = vec2.RotateY();
      rotResult.Add(MatrixRotation.RotateY);
    }
    else if (Math.Abs(vec1.Y) == Math.Abs(vec2.Z)) {
      vec2 = vec2.RotateX();
      rotResult.Add(MatrixRotation.RotateX);
    }
  }

  while (!IsSameRotation(vec1, vec2)) {
    if (IsSameX(vec1, vec2)) {
      vec2 = vec2.RotateX();
      rotResult.Add(MatrixRotation.RotateX);
    }
    else if (IsSameY(vec1, vec2)) {
      vec2 = vec2.RotateY();
      rotResult.Add(MatrixRotation.RotateY);
    }
    else if (IsSameZ(vec1, vec2)) {
      vec2 = vec2.RotateZ();
      rotResult.Add(MatrixRotation.RotateZ);
    }
  }

  return rotResult;
}

Point3D Add(Point3D p1, Point3D p2) {
  return new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
}

Point3D Subtract(Point3D p1, Point3D p2) {
  return new Point3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
}

bool IsSameX(Point3D vec1, Point3D vec2) {
  return vec1.X == vec2.X;
}

bool IsSameY(Point3D vec1, Point3D vec2) {
  return vec1.Y == vec2.Y;
}

bool IsSameZ(Point3D vec1, Point3D vec2) {
  return vec1.Z == vec2.Z;
}

bool IsSameRotation(Point3D vec1, Point3D vec2) {
  return vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z;
}

void CreateNeighbors(IDictionary<int, IList<Point3D>> points) {
  foreach (var key in points.Keys) {
    CreateNeighborList(points[key]);
  }
}

double GetDistance(Point3D p1, Point3D p2) {
  return Math.Sqrt(Math.Pow(Math.Abs(p2.X - p1.X), 2) +
                   Math.Pow(Math.Abs(p2.Y - p1.Y), 2) +
                   Math.Pow(Math.Abs(p2.Z - p1.Z), 2));
}

void CreateNeighborList(IList<Point3D> points) {
  foreach (var p in points) {
    p.Neighbors = points
      .Where(x => !Equals(x, p))
      .Select(x => new {
        Point = x,
        Distance = GetDistance(p, x),
        Minimal = new[] { Math.Abs(p.X - x.X), Math.Abs(p.Y - x.Y), Math.Abs(p.Z - x.Z) }.Min()
      })
      .OrderBy(x => x.Distance)
      .ThenBy(x => x.Minimal)
      .ToDictionary(x => x.Point, x => (x.Distance, x.Minimal));
  }
}


Dictionary<int, IList<Point3D>> ReadInput(IEnumerable<string> inp) {
  const string regex = "--- scanner (?<id>[0-9]+) ---";
  var readInputResult = new Dictionary<int, IList<Point3D>>();
  var scannerId = -1;
  foreach (var line in inp) {
    if (string.IsNullOrWhiteSpace(line)) continue;
    if (Regex.IsMatch(line, regex)) {
      scannerId = int.Parse(Regex.Match(line, regex).Groups["id"].Value);
    }
    else {
      var split = line.Split(",").Select(int.Parse).ToList();
      readInputResult[scannerId] = readInputResult.GetValueOrDefault(scannerId, new List<Point3D>());
      readInputResult[scannerId].Add(new Point3D(split[0], split[1], split[2]));
    }
  }

  return readInputResult;
}

internal static class MatrixRotation {
  public static Point3D RotateX(this Point3D point) {
    return new Point3D(
      point.X,
      -point.Z,
      point.Y);
  }

  public static Point3D RotateY(this Point3D point) {
    return new Point3D(
      point.Z,
      point.Y,
      -point.X);
  }

  public static Point3D RotateZ(this Point3D point) {
    return new Point3D(
      -point.Y,
      point.X,
      point.Z);
  }
}

internal class Point3D {
  public readonly int X;
  public readonly int Y;
  public readonly int Z;

  public Dictionary<Point3D, (double, int)> Neighbors;

  public Point3D(int x, int y, int z) {
    X = x;
    Y = y;
    Z = z;
    Neighbors = new Dictionary<Point3D, (double, int)>();
  }

  private bool Equals(Point3D other) {
    return X == other.X && Y == other.Y && Z == other.Z;
  }

  public override bool Equals(object? obj) {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    return obj.GetType() == this.GetType() && Equals((Point3D)obj);
  }

  public override int GetHashCode() {
    return HashCode.Combine(X, Y, Z);
  }

  public override string ToString() {
    return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";
  }
}