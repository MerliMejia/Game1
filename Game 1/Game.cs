using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Game_1;

public class Game : GameWindow
{
    float[] vertices = {
        -0.5f, -0.5f, 0.0f, //Bottom-left vertex
        0.5f, -0.5f, 0.0f, //Bottom-right vertex
        0.0f,  0.5f, 0.0f  //Top vertex
    };
    
    float speed = 1.5f;

    Vector3 position = new Vector3(0.0f, 0.0f,  3.0f);
    Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
    Vector3 up = new Vector3(0.0f, 1.0f,  0.0f);
    private float width;
    float height;
    
    Matrix4 model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(0f));
    
    int VertexBufferObject;
    private int VertexArrayObject;
    Shader shader;
    Camera camera;
    
    public Game(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings() { Size = (width, height), Title = title })
    {
        this.width = width;
        this.height = height;
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        camera = new Camera(position, width / height);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        shader = new Shader("shader.vert", "shader.frag");
        shader.SetMatrix4("model", model);
        shader.SetMatrix4("view", camera.GetViewMatrix());
        shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        shader.Use();
        
        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
        
        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        KeyboardState input = KeyboardState;
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
        
        // Camera stuff
        if (input.IsKeyDown(Keys.W))
        {
            position += front * speed * (float)e.Time; //Forward 
        }

        if (input.IsKeyDown(Keys.S))
        {
            position -= front * speed * (float)e.Time; //Backwards
        }

        if (input.IsKeyDown(Keys.A))
        {
            position -= Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time; //Left
        }

        if (input.IsKeyDown(Keys.D))
        {
            position += Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time; //Right
        }

        if (input.IsKeyDown(Keys.Space))
        {
            position += up * speed * (float)e.Time; //Up 
        }

        if (input.IsKeyDown(Keys.LeftShift))
        {
            position -= up * speed * (float)e.Time; //Down
        } 
        
        camera.Position = position;
        shader.SetMatrix4("model", model);
        shader.SetMatrix4("view", camera.GetViewMatrix());
        shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        
        GL.Clear(ClearBufferMask.ColorBufferBit);

        shader.Use();
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        
        SwapBuffers();
    }
}