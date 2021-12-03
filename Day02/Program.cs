using System;
using System.IO;
using System.Linq;

var input = File.ReadAllLines("input.txt")
  .Select(x => x.Split(" "))
  .Select(x => new Command((Direction)Enum.Parse(typeof(Direction), x[0]), int.Parse(x[1])))
  .ToList();

var position = new PosAim();

foreach (var command in input) {
  switch (command.Direction) {
    case Direction.forward: position.X += command.Value;
      break;
    case Direction.down: position.Y += command.Value; break;
    case Direction.up: position.Y -= command.Value; break;
  }
}

var result = position.X * position.Y;
Console.Out.WriteLine(result);

var pos2 = new PosAim();

foreach (var command in input) {
  switch (command.Direction) {
    case Direction.forward: 
      pos2.X += command.Value;
      pos2.Y += pos2.Aim * command.Value; 
      break;
    case Direction.down: 
      pos2.Aim += command.Value; 
      break;
    case Direction.up: 
      pos2.Aim -= command.Value; 
      break;
  }
}

var result2 = pos2.X * pos2.Y;
Console.Out.WriteLine(result2);


internal class PosAim {
  public long Aim;
  public long X;
  public long Y;

  public PosAim() {
    Aim = 0;
    X = 0;
    Y = 0;
  }
}

class Command {
  public readonly Direction Direction;
  public readonly int Value;
  public Command(Direction direction, int value) {
    Direction = direction;
    Value = value;
  }
}

internal enum Direction {
  forward,
  up,
  down
}

