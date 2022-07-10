var random = new Random();
var board = new Tile[20, 80];
var allTilePossibilites = Enum.GetValues<TileType>().ToArray();

for (var y = 0; y < board.GetLength(0); y++)
{
    for (var x = 0; x < board.GetLength(1); x++)
    {
        board[y, x] = new Tile(y, x);
    }
}

var initialY = board.GetLength(0)/2;
var initialX = board.GetLength(1)/2;
ref var initial = ref board[initialY, initialX];
initial.SetTile(GetRandomTileType(random));
ReducePossibleNeighboursTiles(board, initialY, initialX);

while (Flatten(board).Any(x => x.TileType == TileType.None))
{
    var min = Flatten(board)
        .Where(x => x.TileType == TileType.None)
        .GroupBy(x => x.NumberOfPossibleTiles)
        .MinBy(x => x.Key)!
        .ToArray();

    var n = min[(new Random()).Next(0, min.Length)];
    n.SetTile(GetRandomTileType(random, n.PossibleTiles));
    ReducePossibleNeighboursTiles(board, n.Y, n.X);
}

Console.Clear();
DrawBoard(board);


IEnumerable<T> Flatten<T>(T[,] map) {
  for (int row = 0; row < map.GetLength(0); row++) {
    for (int col = 0; col < map.GetLength(1); col++) {
      yield return map[row,col];
    }
  }
}

void DrawBoard(Tile[,] tiles)
{
    Console.ResetColor();
    for (var y = 0; y < tiles.GetLength(0); y++)
    {
        for (var x = 0; x < tiles.GetLength(1); x++)
        {
            Console.Write(tiles[y, x].ToString() ?? ".");
        }
        Console.ResetColor();
        Console.WriteLine();
    }

    Console.ResetColor();
}

TileType GetRandomTileType(Random r, TileType[]? bounds = null)
{
    var values = Enum.GetValues<TileType>().Except(new[]{TileType.None})!.ToArray();

    if (bounds != null)
        values = values.Intersect(bounds).ToArray();

    if (values.Length <= 0)
        throw new Exception("Sem combinações possíveis");

    return values[r.Next(values.Length)];
}

void ReducePossibleNeighboursTiles(Tile[,] b, int y, int x)
{
    var possibilies = b[y, x].GetPossibleNeighbours();

    GetTile(y - 1, x - 1)?.ReducePossibleTiles(possibilies);
    GetTile(y - 1, x)?.ReducePossibleTiles(possibilies);
    GetTile(y - 1, x + 1)?.ReducePossibleTiles(possibilies);

    GetTile(y, x - 1)?.ReducePossibleTiles(possibilies);
    GetTile(y, x + 1)?.ReducePossibleTiles(possibilies);

    GetTile(y + 1, x - 1)?.ReducePossibleTiles(possibilies);
    GetTile(y + 1, x)?.ReducePossibleTiles(possibilies);
    GetTile(y + 1, x + 1)?.ReducePossibleTiles(possibilies);

    Tile? GetTile(int cy, int cx)
    {
        if (cy < 0 || cy >= b.GetLength(0) || cx < 0 || cx >= b.GetLength(1))
            return null;

        return b[cy, cx];
    }
}

public enum TileType { None, Mountain, Grass, Sand, Water, }

public class Tile
{
    public TileType TileType { get; private set; }
    public TileType[] PossibleTiles { get; private set; }
    public int Y { get; }
    public int X { get; }
    public int NumberOfPossibleTiles => PossibleTiles.Length;

    public Tile(int y, int x)
    {
        TileType = TileType.None;
        PossibleTiles = Enum.GetValues<TileType>().Except(new[] {TileType.None}).ToArray();
        Y = y;
        X = x;
    }

    public TileType[] GetPossibleNeighbours()
    {
        return TileType switch
        {
            TileType.Mountain => new[] { TileType.Grass, TileType.Mountain },
            TileType.Grass => new[] { TileType.Mountain, TileType.Sand, TileType.Sand, TileType.Grass },
            TileType.Sand => new[] { TileType.Water, TileType.Grass, TileType.Sand},
            TileType.Water => new[] { TileType.Sand, TileType.Grass, TileType.Water},
            TileType.None => Enum.GetValues<TileType>().Except(new[] {TileType.None}).ToArray(),
            _ => throw new Exception($"TileType {TileType} not implemented"),
        };
    }

    public void ReducePossibleTiles(TileType[] types)
    {
        try
        {
            if (types != null && types.Length > 0)
                PossibleTiles = PossibleTiles.Intersect(types).ToArray();

        }
        catch (Exception)
        {
            Console.WriteLine($"PossibleTiles {PossibleTiles?.Length ?? -1}, Types: {types?.Length ?? -1}");
        }
    }

    public void SetTile(TileType type)
    {
        if (TileType != TileType.None)
            throw new Exception($"TileType already set: {TileType}");

        if (type == TileType.None)
            throw new Exception($"Cannot set TileType to None");

        TileType = type;
        PossibleTiles = Enumerable.Empty<TileType>().ToArray();
    }

    public override string ToString()
    {
        switch (TileType)
        {
            case TileType.Mountain:
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                return "M";
            case TileType.Grass:
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                return "G";
            case TileType.Sand:
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                return "S";
            case TileType.Water:
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.Blue;
                return "W";
            case TileType.None:
                Console.ResetColor();
                return " ";
            default:
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Red;
                return "E";
        }
    }
}