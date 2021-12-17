using System.Drawing;

/*const int minX = 20;
const int maxX = 30;
const int minY = -10;
const int maxY = -5;
const int maxV = 40;*/

const int minX = 143;
const int maxX = 177;
const int minY = -106;
const int maxY = -71;
const int maxV = 1000;

var maximumY = 0;

var initV = new HashSet<Point>();

for (var xv = 1; xv < maxV; xv++) {
  for (var yv = -maxV; yv < maxV; yv++) {
    var res = CalcTrajectory(xv, yv);
    if (res.Any()) {
      var newMaxY = res.Max(p => p.Y);
      if (newMaxY > maximumY) maximumY = newMaxY;
      initV.Add(new Point(xv, yv));
    }
  }
}

Console.Out.WriteLine(maximumY);
Console.Out.WriteLine(initV.Count);

IList<Point> CalcTrajectory(int xv, int yv) {
  var pos = new Point(0, 0);
  var list = new List<Point>();
  while (pos.X <= maxX && pos.Y >= minY) {
    list.Add(pos);
    if (IsHit(pos)) {
      return list;
    }
    pos.X += xv;
    pos.Y += yv;
    xv = xv > 0 ? xv-1 : 0;
    yv--;
  }
  return new List<Point>();
}

bool IsHit(Point pos) {
  return pos.X is >= minX and <= maxX && pos.Y is >= minY and <= maxY;
}