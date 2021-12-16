using System.Text;

var input = File.ReadAllText("input.txt")
  .Select(x => x.ToString())
  .Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'))
  .Aggregate((s, s1) => s + s1);
  

var reader = new Reader(input);

long versionSum = 0;

while (!reader.EoF()) {
  var (version, typeId) = ReadHeader(reader);
  versionSum += version;
  if (typeId == 4) {
    var literal = ReadLiteral(reader);
  }
  else {
    var length = ReadOperator(reader);
    reader.Read(length);
  }
}

Console.Out.WriteLine(versionSum);

int ReadOperator(Reader r) {
  var lengthOfSubs = r.Read() == "0" ? 15 : 11;
  return lengthOfSubs;
  var length = Convert.ToInt64(r.Read(lengthOfSubs));
  var counter = 0L;
  IEnumerable<long> literals;
  while (counter < length) {
    var (lit, litlen) = ReadLiteral(r);
    counter += litlen;
  }

  return (int)(counter + 1L);
}

(long, long) ReadHeader(Reader r) {
  var v = Convert.ToInt32(r.Read(3), 2);
  var t = Convert.ToInt32(r.Read(3), 2);
  return (v, t);
}

(long, long) ReadLiteral(Reader r) {
  SkipZeros(r);
  var number = "";
  var finished = false;
  while (!finished) {
    var isLast = r.Read() == "0";
    number += r.Read(4);    
    finished = isLast;
  }
  return (Convert.ToInt64(number, 2), number.Length);
}

void SkipZeros(Reader r) {
  while (r.Peek() == "0") {
    r.Read();
  }
}

class Reader {
  private int _position;
  private readonly string _input;

  public Reader(string input) {
    _input = input;
    _position = 0;
  }

  public bool EoF() {
    return _position == _input.Length;
  }
  
  public string Read(int count = 1) {
    if (EoF()) return string.Empty;
    var value = _input.Substring(_position, count);
    _position += count;
    return value;
  }

  public string Peek(int count = 0) {
    return EoF() ? string.Empty : _input.Substring(_position, count);
  }
}