using System;
using System.IO;
using System.Linq;

var input = File.ReadAllLines("input.txt").Select(x => x.ToArray()).ToList();

var len = input.First().Length;

var gamma = new char[len];
var epsilon = new char[len];

for (var i = 0; i < len; i++) {
  var gammaCount = input.Count(x => x[i] == '1');
  var epsilonCount = input.Count - gammaCount;

  if (gammaCount > epsilonCount) {
    gamma[i] = '1';
    epsilon[i] = '0';
  }

  if (gammaCount < epsilonCount) {
    gamma[i] = '0';
    epsilon[i] = '1';
  }
}

var gammaValue = Convert.ToInt32(new string(gamma), 2);
var epsilonValue = Convert.ToInt32(new string(epsilon), 2);

Console.Out.WriteLine(gammaValue * epsilonValue);

var list = input.ToList();
var pos = 0;
while (list.Count > 1) {
  var gammaCount = list.Count(x => x[pos] == '1');
  var epsilonCount = list.Count - gammaCount;

  if (gammaCount > epsilonCount) {
    list = list.Where(x => x[pos] == '1').ToList();
  }
  else if (gammaCount < epsilonCount) {
    list = list.Where(x => x[pos] == '0').ToList();
  }
  else {
    list = list.Where(x => x[pos] == '1').ToList();
  }

  pos++;
}

var ox = Convert.ToInt32(new string(list.Single()), 2);

list = input.ToList();
pos = 0;
while (list.Count > 1) {
  var gammaCount = list.Count(x => x[pos] == '1');
  var epsilonCount = list.Count - gammaCount;

  if (gammaCount > epsilonCount) {
    list = list.Where(x => x[pos] == '0').ToList();
  }
  else if (gammaCount < epsilonCount) {
    list = list.Where(x => x[pos] == '1').ToList();
  }
  else {
    list = list.Where(x => x[pos] == '0').ToList();
  }

  pos++;
}

var co2 = Convert.ToInt32(new string(list.Single()), 2);

Console.Out.WriteLine(co2 * ox);