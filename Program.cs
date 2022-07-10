var random = new Random();
var board = new Tile[20, 80];
var allTilePossibilites = Enum.GetValues<TileType>().ToArray();

var initialY = board.GetLength(0)/2;
var initialX = board.GetLength(1)/2;
ref var initial = ref board[initialY, initialX];
initial.SetTile(GetRandomTileType(random));

for (var y = 0; y < board.GetLength(0); y++)
{
    for (var x = 0; x < board.GetLength(1); x++)
    {
        if (initialY == 0 && initialX == 0) continue;

        var possibleTiles = GetTileTypePossibilities(board, y, x);
        var newTile = CreateRandomTile(random, possibleTiles);

        if (newTile == null)
        {
            System.Console.WriteLine($"Sem combinações possívels y={y} x={x}");
            DrawBoard(board);
            return;
        }

        board[y, x] = newTile;
    }
}

DrawBoard(board);

void DrawBoard(Tile[,] tiles)
{
    for (var y = 0; y < tiles.GetLength(0); y++)
    {
        for (var x = 0; x < tiles.GetLength(1); x++)
        {
            Console.Write(tiles[y, x]?.ToString() ?? ".");
        }
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

TileType[] GetTileTypePossibilities(Tile[,] tiles, int y, int x)
{
    return
        GetTileNeighbours(y - 1, x - 1)
        .Intersect(GetTileNeighbours(y - 1, x))
        .Intersect(GetTileNeighbours(y - 1, x + 1))
        .Intersect(GetTileNeighbours(y, x - 1))
        .Intersect(GetTileNeighbours(y, x))
        .Intersect(GetTileNeighbours(y, x + 1))
        .Intersect(GetTileNeighbours(y + 1, x - 1))
        .Intersect(GetTileNeighbours(y + 1, x))
        .Intersect(GetTileNeighbours(y + 1, x + 1))
        .ToArray();

    TileType[] GetTileNeighbours(int cy, int cx)
    {
        if (cy < 0 || cy >= tiles.GetLength(0) || cx < 0 || cx >= tiles.GetLength(1))
            return allTilePossibilites;

        return tiles[cy, cx]?.GetPossibleNeighbours() ?? allTilePossibilites;
    }
}

public enum TileType { None, Mountain, Grass, Sand, Water, }

public struct Tile
{
    public TileType TileType { get; private set; }
    public TileType[] PossibleTiles { get; private set; }
    public int NumberOfPossibleTiles => PossibleTiles.Length;

    public Tile()
    {
        TileType = TileType.None;
        PossibleTiles = Enum.GetValues<TileType>().Except(new[] {TileType.None}).ToArray();
    }

    public TileType[] GetPossibleNeighbours()
    {
        return TileType switch
        {
            TileType.Mountain => new[] { TileType.Grass, TileType.Mountain},
            TileType.Grass => new[] { TileType.Mountain, TileType.Sand, TileType.Sand, TileType.Grass },
            TileType.Sand => new[] { TileType.Water, TileType.Grass, TileType.Sand},
            TileType.Water => new[] { TileType.Sand, TileType.Grass, TileType.Water},
            TileType.None => Enum.GetValues<TileType>().Except(new[] {TileType.None}).ToArray(),
            _ => throw new Exception($"TileType {TileType} not implemented"),
        };
    }

    public void ReducePossibleTiles(TileType[] types)
    {
        PossibleTiles = PossibleTiles.Intersect(types).ToArray();
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
            default:
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Red;
                return "E";
        }
    }
}