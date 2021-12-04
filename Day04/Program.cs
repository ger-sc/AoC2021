using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var input = File.ReadAllLines("input.txt");

var numbers = input.First().Split(",").Select(int.Parse).ToList();

var boards = new List<Board>();
var line = 2;
var currentBoardIndex = 0;
var currentBoard = new Board(currentBoardIndex);
var row = 0; 
while (line <= input.Length) {
  if (line == input.Length) {
    boards.Add(currentBoard);
    line++;
  } else if (string.IsNullOrWhiteSpace(input[line])) {
    line++;
    boards.Add(currentBoard);
    currentBoardIndex++;
    currentBoard = new Board(currentBoardIndex);
    row = 0;
  } else {
    var values = input[line].Trim().Replace("  ", " ").Split(" ");
    currentBoard.Items.AddRange(values.Select((x, i) => new BoardItem(i, row, int.Parse(x))));
    row++;
    line++;
  }
}

var currentIdx = -1;
while (!boards.Any(HasWon)) {
  currentIdx++;
  var number = numbers[currentIdx];
  foreach (var item in boards.Select(board => board.Items.Where(x => x.Value == number)).SelectMany(items => items)) {
    item.Visited = true;
  }
}

var winner = boards.Where(HasWon).Single();

var result = winner.Items.Where(x => !x.Visited).Sum(x => x.Value) * numbers[currentIdx];

Console.Out.WriteLine(result);

foreach (var item in boards.SelectMany(x => x.Items)) {
  item.Visited = false;
}

currentIdx = -1;
var winners = new List<Board>();
while (boards.Count(HasWon) < boards.Count) {
  currentIdx++;
  var number = numbers[currentIdx];
  foreach (var board in boards) {
    var items = board.Items.Where(x => x.Value == number);
    foreach (var item in items) {
      item.Visited = true;
    }
    if (HasWon(board) && winners.All(x => x.Id != board.Id)) {
      winners.Add(board);
    }
  }
}

var lastWinner = winners.Last();

var result2 = lastWinner.Items.Where(x => !x.Visited).Sum(x => x.Value) * numbers[currentIdx];

Console.Out.WriteLine(result2);

bool HasWon(Board board) {
  return board.Items.GroupBy(x => x.Row).Any(x => x.All(r => r.Visited))
  || board.Items.GroupBy(x => x.Column).Any(x => x.All(r => r.Visited));
}

internal class Board {
  public readonly int Id;
  public readonly List<BoardItem> Items;

  public Board(int id) {
    Id = id;
    Items = new List<BoardItem>();
  }
}
internal class BoardItem {
  public readonly int Column;
  public readonly int Row;
  public readonly int Value;
  public bool Visited;

  public BoardItem(int column, int row, int value) {
    Column = column;
    Row = row;
    Value = value;
    Visited = false;
  }
}