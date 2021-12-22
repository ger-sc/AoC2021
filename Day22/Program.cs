using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

const string regex = @"(?<mode>on|off) x=(?<xmin>-?\d+)..(?<xmax>-?\d+),y=(?<ymin>-?\d+)..(?<ymax>-?\d+),z=(?<zmin>-?\d+)..(?<zmax>-?\d+)";

var instructions = input.Select(x => {
  var match = Regex.Match(x, regex);
  var mode = match.Groups["mode"].Value;
  var xMin = int.Parse(match.Groups["xmin"].Value);
  var xMax = int.Parse(match.Groups["xmax"].Value);
  var yMin = int.Parse(match.Groups["ymin"].Value);
  var yMax = int.Parse(match.Groups["ymax"].Value);
  var zMin = int.Parse(match.Groups["zmin"].Value);
  var zMax = int.Parse(match.Groups["zmax"].Value);
  return new Instruction(mode == "on", new Cuboid(xMin, xMax, yMin, yMax, zMin, zMax));
}).ToList();

var dict = new Dictionary<(long, long, long), bool>();

foreach (var i in instructions) {
  const int limitMin = -50;
  const int limitMax = 50;

  for (var z = Math.Max(i.Cuboid.ZMin, limitMin); z <= Math.Min(i.Cuboid.ZMax, limitMax); z++) {
    for (var y = Math.Max(i.Cuboid.YMin, limitMin); y <= Math.Min(i.Cuboid.YMax, limitMax); y++) {
      for (var x = Math.Max(i.Cuboid.XMin, limitMin); x <= Math.Min(i.Cuboid.XMax, limitMax); x++) {
        var point = (x, y, z);
        if (i.IsOn) {
          dict[point] = true;
        } else {
          dict.Remove(point);
        }
      }
    }
  }
}

Console.Out.WriteLine(dict.Count);

var allCuboids = new Dictionary<Cuboid, long>();
foreach (var step in instructions) {
  var newCubes = new Dictionary<Cuboid, long>();

  foreach (var (key, value) in allCuboids) {
    var intersect = GetIntersect(key, step.Cuboid);
    if (intersect != null) {
      newCubes[intersect.Value] = newCubes.GetValueOrDefault(intersect.Value, 0) - value;
    }
  }

  if (step.IsOn) {
    newCubes[step.Cuboid] = newCubes.GetValueOrDefault(step.Cuboid, 0) + (step.IsOn ? 1 : -1);
  }

  foreach (var (key, value) in newCubes) {
    allCuboids[key] = allCuboids.GetValueOrDefault(key, 0) + value;
  }
}

Console.Out.WriteLine(allCuboids.Sum(a => (a.Key.XMax - a.Key.XMin + 1L) * (a.Key.YMax - a.Key.YMin + 1) * (a.Key.ZMax - a.Key.ZMin + 1) * a.Value));


Cuboid? GetIntersect(Cuboid fst, Cuboid snd) {
  if (fst.XMin > snd.XMax || fst.XMax < snd.XMin || 
      fst.YMin > snd.YMax || fst.YMax < snd.YMin || 
      fst.ZMin > snd.ZMax || fst.ZMax < snd.ZMin) {
    return null;
  }
  return new Cuboid(
    Math.Max(fst.XMin, snd.XMin),
    Math.Min(fst.XMax, snd.XMax),
    Math.Max(fst.YMin, snd.YMin),
    Math.Min(fst.YMax, snd.YMax),
    Math.Max(fst.ZMin, snd.ZMin),
    Math.Min(fst.ZMax, snd.ZMax));
}

internal readonly struct Cuboid {

  public readonly long XMin;
  public readonly long XMax;
  public readonly long YMin;
  public readonly long YMax;
  public readonly long ZMin;
  public readonly long ZMax;

  public Cuboid(long xMin, long xMax, long yMin, long yMax, long zMin, long zMax) {
    XMin = xMin;
    XMax = xMax;
    YMin = yMin;
    YMax = yMax;
    ZMin = zMin;
    ZMax = zMax;
  }

  public override string ToString() {
    return $"{XMin}..{XMax}, {YMin}..{YMax}, {ZMin}..{ZMax}";
  }
}

internal readonly struct Instruction {
  public readonly bool IsOn;
  public readonly Cuboid Cuboid;

  public Instruction(bool isOn, Cuboid cuboid) {
    IsOn = isOn;
    Cuboid = cuboid;
  }

  public override string ToString() {
    return $"{IsOn} {Cuboid}";
  }
}