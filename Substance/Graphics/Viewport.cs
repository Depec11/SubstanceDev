using Substance.Maths;

namespace Substance.Graphics;

public class Viewport
{
    public static Viewport Current => _current;
    private static Viewport _current = null!;

    public Vector2<float> Position { get; set
        {
            if (value == field)
            {
                return;
            }

            field = value;

            Update();
        } } = Vector2<float>.Zero;
    public Vector2<float> Size { get; set
        {
            if (value == field)
            {
                return;
            }

            field = value;

            Update();
        } } = Vector2<float>.One;
    public float Rotation { get; set
        {
            if (value == field)
            {
                return;
            }

            field = value;

            Update();
        } } = 0;

    private Matrix3x2 _vm;
    private Matrix3x2 _pm;
    private Matrix3x2 _vp;
    private Matrix3x2 _svp;

    public Viewport()
    {
        _current = this;

        Size = new Vector2<float>(Application.MainWindow.Size.X, Application.MainWindow.Size.Y);

        Update();
    }

    public Matrix3x2 GetMvp(Matrix3x2 om)
    {
        return om * _vp;
    }

    public Matrix3x2 GetSvp(Matrix3x2 os)
    {
        return os * _svp;
    }

    private void Update()
    {
        _vm = Matrix3x2.Make(Position, Rotation, Size);

        var halfWidth = Size.X / 2.0f;
        var halfHeight = Size.Y / 2.0f;

        _pm = new Matrix3x2(
            1 / halfWidth,  0,
            0,             -1 / halfHeight,
            0,              0
        );

        _vp = _vm * _pm;

        _svp = new Matrix3x2(
             1 / halfWidth,  0,
             0,             -1 / halfHeight,
            -1,              1
        );
    }
}