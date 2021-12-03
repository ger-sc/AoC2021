using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var input = File.ReadAllLines("input.txt").Select(int.Parse).ToList();
Part1(input);
Part2(input);

void Part1(IEnumerable<int> depths) {
  var list = depths.ToList();
  var count = list
    .Zip(list.Skip(1))
    .Count(x => x.First < x.Second);
  Console.Out.WriteLine(count);
}

void Part2(IEnumerable<int> depths) {
  var list = depths.ToList();
  var newList = new List<int>();
  for (var i = 0; i < list.Count - 2; i++) {
    newList.Add(list[i]+list[i+1]+list[i+2]);
  }
  var count = newList
    .Zip(newList.Skip(1))
    .Count(x => x.First < x.Second);
  Console.Out.WriteLine(count);
}