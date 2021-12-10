using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

var input = File.ReadAllLines("input.txt");

var result = input
  .Select(GetFirstIncorrectClosing)
  .ToList();
  
var part1 = result
  .Select(x => x.Item1)
  .Select(GetValue)
  .Sum();

Console.Out.WriteLine(part1);

var part2 =result
  .Where(x => x.Item1 is null)
  .Select(x => GetString(x.Item2))
  .Where(x => !string.IsNullOrWhiteSpace(x))
  .Select(CalcPart2)
  .OrderBy(x => x)
  .ToList();

Console.Out.WriteLine(part2[part2.Count / 2]);

(char?,Stack<char>) GetFirstIncorrectClosing(string s) {
  var stack = new Stack<char>();
  foreach (var t in s) {
    if (IsOpening(t)) {
      stack.Push(t);
    }
    else {
      if (t != GetEnd(stack.Pop())) {
        return (t, stack);
      }
    }
  }
  return (null, stack);
}

char GetEnd(char c) {
  return c switch {
    '(' => ')',
    '[' => ']',
    '{' => '}',
    '<' => '>',
    _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
  };
}

long GetValue(char? c) {
  if (!c.HasValue) return 0;
  return c.Value switch {
    ')' => 3,
    ']' => 57,
    '}' => 1197,
    '>' => 25137,
    _ => 0
  };
}

string GetString(Stack<char> stack) {
  var sb = new StringBuilder();
  while (stack.Any()) {
    sb.Append(GetEnd(stack.Pop()));
  }
  return sb.ToString();
}

long CalcPart2(string s) {
  long sum = 0;
  foreach (var t in s) {
    sum *= 5;
    sum += GetValue2(t);
  }
  return sum;
}

int GetValue2(char c) {
  return c switch {
    ')' => 1,
    ']' => 2,
    '}' => 3,
    '>' => 4,
    _ => 1
  };
}

bool IsOpening(char c) {
  return c is '{' or '[' or '<' or '(';
}