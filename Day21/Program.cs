const int player1Start = 6;
const int player2Start = 8;

var player1Index = player1Start - 1;
var player2Index = player2Start - 1;

var scorePlayer1 = 0;
var scorePlayer2 = 0;

var dice = new DeterministicDice();

while (true) {
  var player1Roll = Enumerable.Range(1, 3).Select(_ => dice.Roll()).Sum();
  player1Index = (player1Index + player1Roll) % 10;
  scorePlayer1 += player1Index + 1;
  if (scorePlayer1 >= 1000) break;
  var player2Roll = Enumerable.Range(1, 3).Select(_ => dice.Roll()).Sum();
  player2Index = (player2Index + player2Roll) % 10;
  scorePlayer2 += player2Index + 1;
  if (scorePlayer2 >= 1000) break;
}

Console.Out.WriteLine(dice.Counter * (scorePlayer1 < 1000 ? scorePlayer1 : scorePlayer2));

player1Index = player1Start - 1;
player2Index = player2Start - 1;

scorePlayer1 = 0;
scorePlayer2 = 0;

long player1Wins = 0;
long player2Wins = 0;

var distribution = new Dictionary<int, int> {
  { 3, 1 },
  { 4, 3 },
  { 5, 6 },
  { 6, 7 },
  { 7, 6 },
  { 8, 3 },
  { 9, 1 }
};

QuantumDie(player1Index, player2Index, scorePlayer1, scorePlayer2, true, 1, ref player1Wins, ref player2Wins);

Console.Out.WriteLine(player1Wins > player2Wins ? player1Wins : player2Wins);

void QuantumDie(int p1Pos, int p2Pos, int p1Score, int p2Score, bool p1Turn, long universes, ref long p1Wins, ref long p2Wins) {
  if (p1Turn) {
    for (var rollSum = 3; rollSum <= 9; rollSum++) {
      var newP1Pos = p1Pos;
      newP1Pos = (newP1Pos + rollSum) % 10;
      var newP1Score = p1Score + newP1Pos + 1;
      var newUniverses = universes * distribution[rollSum];
      if (newP1Score >= 21) {
        p1Wins += newUniverses;
      } else {
        QuantumDie(newP1Pos, p2Pos, newP1Score, p2Score, false, newUniverses, ref p1Wins, ref p2Wins);
      }
    }
  } else {
    for (var rollSum = 3; rollSum <= 9; rollSum++) {
      var newP2Pos = p2Pos;
      newP2Pos = (newP2Pos + rollSum) % 10;
      var newP2Score = p2Score + newP2Pos + 1;
      if (newP2Score >= 21) {
        p2Wins += universes * distribution[rollSum];
      } else {
        QuantumDie(p1Pos, newP2Pos, p1Score, newP2Score, true, universes * distribution[rollSum], ref p1Wins, ref p2Wins);
      }
    }
  }
}

internal class DeterministicDice {
  private int _currentValue;
  public int Counter;
  public int Roll() {
    Counter++;
    _currentValue += 1;
    if (_currentValue > 100) {
      _currentValue = 1;
    }
    return _currentValue;
  }
}