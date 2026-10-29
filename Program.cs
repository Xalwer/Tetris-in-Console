using System.Text;

class Program
{
    static public void Main()
    {
        Console.CursorVisible = false;
        Map map = new Map();
        Control ctrl = new(map);
        Thread thread = new Thread(ctrl.Catch);
        thread.Start();
        map.Start();
    }
}
class Control
{
    Map map;
    public Control(Map map)
    {
        this.map = map;
    }
    public void Catch()
    {
        while(true)
        {
            map.KeyHandler(Console.ReadKey());
        }
    }
}
class Map
{
    public int Speed { get; set; }
    public int Rows { get; private set; }
    public Figure CurrentFigure { set; get; }
    public bool[][] pixels = new bool[20][];
    public Map()
    {

        for (int i = 0; i < 20; i++)
        {
            pixels[i] = new bool[10];
        }
        
    }
    public void Start()
    {
        Thread thread = new Thread(() => Render(5));
        thread.Start();
        while (true)
        {
            Update();
        }
    }
    public bool KeyHandler(ConsoleKeyInfo key)
    {
        bool result = false;
        foreach (var square in CurrentFigure.squares)
        {
            pixels[square.Y][square.X] = false;
        }

        if (key.Key == ConsoleKey.UpArrow) result = CurrentFigure.Rotate(this);
        else if (key.Key == ConsoleKey.RightArrow) result = CurrentFigure.StepRight(this);
        else if (key.Key == ConsoleKey.LeftArrow) result = CurrentFigure.StepLeft(this);
        else if (key.Key == ConsoleKey.DownArrow) result = CurrentFigure.StepDown(this);

        foreach (var square in CurrentFigure.squares)
        {
            pixels[square.Y][square.X] = true;
        }
        return result;
    }
    void Render(int delay)
    {
        while (true)
        {
            Thread.Sleep(delay);
            Draw.DrawToConsole(this);
        }
        
    }
    void Update()
    {
        Clear();
        while (Frame()) ;
    }
    bool Frame()
    {
        Figure figure = new Figure();
        figure.Generate(2, 4);
        CurrentFigure = figure;
        foreach (var square in figure.squares)
        {
            if (pixels[square.Y][square.X]) return false;
        }
        while (KeyHandler(new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, false, false, false)))
        {
            Thread.Sleep(400);
        }

        for (int i = 0; i < pixels.Length; i++)
        {
            if (CheckFullRow(pixels[i]))
            {
                for (int j = i; j > 0; j--)
                {
                    pixels[j] = pixels[j - 1];
                }
                pixels[0] = new bool[10];
                Rows++;
            }
        }

        return true;
    }
    void Clear()
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            Array.Clear(pixels[i], 0, pixels[i].Length);
        }
        Rows = 0;
    }
    bool CheckFullRow(bool[] row) // All()
    {
        for (int x = 0; x < row.Length; x++)
        {
            if (row[x] == false)
            {
                return false;
            }
        }
        return true;
    }
}
class Figure
{
    public void Generate(int offsetY, int offsetX)
    {
        Random rnd = new Random();
        var figureType = rnd.Next(0,7);
        if (figureType == 0) // -
        {
            squares[0] = new Square() { Y = 0, X = 0 };
            squares[1] = new Square() { Y = 0, X = 1 };
            squares[2] = new Square() { Y = 0, X = 2 };
            squares[3] = new Square() { Y = 0, X = 3 };
        }
        else if (figureType == 1) // L
        {
            squares[0] = new Square() { Y = 0, X = 0 };
            squares[1] = new Square() { Y = 0, X = 1 };
            squares[2] = new Square() { Y = 0, X = 2 };
            squares[3] = new Square() { Y = 1, X = 2 };
        }
        else if (figureType == 2) // T
        {
            squares[0] = new Square() { Y = 1, X = 0 };
            squares[1] = new Square() { Y = 1, X = 1 };
            squares[2] = new Square() { Y = 0, X = 1 };
            squares[3] = new Square() { Y = 1, X = 2 };
        }
        else if (figureType == 3) // N
        {
            squares[0] = new Square() { Y = 0, X = 0 };
            squares[1] = new Square() { Y = 1, X = 0 };
            squares[2] = new Square() { Y = 1, X = 1 };
            squares[3] = new Square() { Y = 2, X = 1 };
        }
        else if (figureType == 4) // O
        {
            squares[0] = new Square() { Y = 0, X = 0 };
            squares[1] = new Square() { Y = 1, X = 0 };
            squares[2] = new Square() { Y = 1, X = 1 };
            squares[3] = new Square() { Y = 0, X = 1 };
        }
        else if (figureType == 5) // Lreverse
        {
            squares[0] = new Square() { Y = 1, X = 0 };
            squares[1] = new Square() { Y = 1, X = 1 };
            squares[2] = new Square() { Y = 1, X = 2 };
            squares[3] = new Square() { Y = 0, X = 2 };
        }
        else if (figureType == 6) // Nreverse
        {
            squares[0] = new Square() { Y = 0, X = 1 };
            squares[1] = new Square() { Y = 1, X = 0 };
            squares[2] = new Square() { Y = 1, X = 1 };
            squares[3] = new Square() { Y = 2, X = 0 };
        }
        for (int i = 0; i < 4; i++)
        {
            squares[i].X += offsetX;
            squares[i].Y += offsetY;
        }
    }
    public Square[] squares = new Square[4];

    public bool StepRight(Map map)
    {

        foreach (var square in squares)
        {
            if (square.X + 1 > map.pixels[0].Length-1) return false;
            if (map.pixels[square.Y][square.X + 1]) return false;
        }
        foreach(var square in squares)
        {
            square.X++;
        }
        return true;
    }
    public bool StepLeft(Map map)
    {
        foreach (var square in squares)
        {
            if (square.X < 1) return false;
            if (map.pixels[square.Y][square.X - 1]) return false;
        }
        foreach (var square in squares)
        {
            square.X--;
        }
        return true;
    }
    public bool StepDown(Map map)
    {
        foreach (var square in squares)
        {
            if (square.Y + 1 > map.pixels.Length-1) return false;
            if (map.pixels[square.Y + 1][square.X]) return false;
        }
        foreach (var square in squares)
        {
            square.Y++;
        }
        return true;
    }
    public bool Rotate(Map map)
    {
        int y,x;
        (int, int)[] coordsList = new (int, int)[4];
        (int centerY, int centerX) = FindCenter();
        for (int i = 0; i < squares.Length; i++)
        {
            int translatedX = squares[i].X - centerX;
            int translatedY = squares[i].Y - centerY;

            y = translatedX + centerY;
            x = -translatedY + centerX;

            if (x < 0 || x > map.pixels[0].Length - 1) return false;
            if (y < 0 || y > map.pixels.Length - 1) return false;
            if (map.pixels[y][x]) return false;

            coordsList[i] = (y, x);
        }
        for (int i = 0; i < squares.Length; i++)
        {
            squares[i].Y = coordsList[i].Item1;
            squares[i].X = coordsList[i].Item2;
        }
        return true;
    }
    (int y, int x) FindCenter()
    {

        int sumX = 0;
        int sumY = 0;

        for (int i = 0; i < squares.Length; i++)
        {
            sumX += squares[i].X;
            sumY += squares[i].Y;
        }

        int centerX = (int)Math.Round((double)sumX / squares.Length);
        int centerY = (int)Math.Round((double)sumY / squares.Length);

        return (centerY,centerX);
    }

}
class Square
{
    public int X { get; set; }
    public int Y { get; set; }
}
class Draw
{
    static StringBuilder _sb = new StringBuilder();
    static string color1 = "\u001b[38;2;255;255;255m";
    static string color2 = "\u001b[38;2;240;240;240m";
    static string color;
    static string reset = "\u001b[0m";
    static public void DrawToConsole(Map map)
    {
        int i = 0;
        try
        {
            
            foreach (bool[] row in map.pixels)
            {
                foreach (bool pixel in row)
                {
                    color = (i % 2) == 0 ? color1 : color2;
                    _sb.Append($"{color}" + (pixel ? "██" : "░░") + $"{reset}");
                    i++;
                }
                _sb.Append("\n");
                i++;
            }
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(_sb);
            Console.WriteLine($"\nRows:\t{map.Rows}");
            Console.WriteLine($"\nSpeed:\t{map.Speed}");
        }
        finally 
        { 
            _sb.Clear(); 
        }
    }
}