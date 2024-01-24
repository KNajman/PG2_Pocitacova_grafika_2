using Final;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace PG2
{
    public class Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : GameWindow(gameWindowSettings, nativeWindowSettings)
    {
        // VAO, VBO, EBO

        private int VBO; // Vertex Buffer Object
        private int vaoModel, vaoSun, vaoLamp;

        //Camera
        private Camera camera;
        private bool firstMove = true;
        private Vector2 lastPos;

        private readonly float[] vertices =
        {
            // Positions          Normals              Texture coords
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
        };

        // Ppoint lights' positions to draw the lamps and to get light the materials properly 
        private readonly Vector3[] pointLightPositions =
        {
            // 4 random point lights in row over the planets
            new( -2.0f, 2.0f, 2.0f),
            new( 3.0f, 2.0f, 2.0f),
            new( 8.0f, 2.0f, 2.0f),
            new( 12.0f, 2.0f, 2.0f),
        };

        private Sphere sun, mercury, earth, moon, venus, mars, jupiter, saturn, uran, neptun;

        private Shader lampShader, lightingShader, sunShader;
        private Texture sunMap, earthMap, moonMap, earthAtmosphere, venusMap, venusAtmosphere, marsMap, mercuryMap, jupiterMap, saturnMap, uranMap, neptunMap;
        private static Stopwatch timer = new();
        private static int fps;
        private bool vsyncEnabled = true;

        protected override void OnLoad()
        {
            base.OnLoad();

            // Enable features we want to use from OpenGL            
            GL.Enable(EnableCap.Texture2D); // Enable Texture Mapping
            GL.Enable(EnableCap.DepthTest);  // Enable depth testing for z-culling

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            // Vertex Buffer Object
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            // GL.EnableVertexAttribArray(1);

            //shaders
            lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            //sunShader = new Shader("Shaders/shader.vert", "Shaders/sun.frag");

            vaoModel = GL.GenVertexArray();
            GL.BindVertexArray(vaoModel);
            SetupShaderAttributes(lightingShader, "aPos", "aNormal", "aTexCoords");

            vaoLamp = GL.GenVertexArray();
            GL.BindVertexArray(vaoLamp);
            SetupShaderAttributes(lampShader, "aPos");

            // vaoSun = GL.GenVertexArray();
            // GL.BindVertexArray(vaoSun);
            // SetupShaderAttributes(sunShader, "aPos", "aNormal", "aTexCoords");

            SetSolarSystem();

            // Initialize the camera
            camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            CursorState = CursorState.Grabbed;

            // Start the timer
            timer = new Stopwatch();
            timer.Start();
        }

        private static void SetupShaderAttributes(Shader shader, params string[] attributeNames)
        {
            for (int i = 0; i < attributeNames.Length; i++)
            {
                var attributeLocation = shader.GetAttribLocation(attributeNames[i]);
                GL.EnableVertexAttribArray(attributeLocation);
                GL.VertexAttribPointer(attributeLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), i * 3 * sizeof(float));
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Set the shader and uniforms for lighting
            GL.BindVertexArray(vaoModel);

            lightingShader.Use();

            lightingShader.SetMatrix4("view", camera.GetViewMatrix());
            lightingShader.SetMatrix4("projection", camera.GetProjectionMatrix());
            lightingShader.SetVector3("viewPos", camera.Position);

            lightingShader.SetInt("material.diffuse", 0);
            lightingShader.SetInt("material.specular", 1);
            lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            lightingShader.SetFloat("material.shininess", 32.0f);

            // Set the properties of the directional light
            lightingShader.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
            lightingShader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
            lightingShader.SetVector3("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
            lightingShader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));

            //Point lights
            for (int i = 0; i < pointLightPositions.Length; i++)
            {
                lightingShader.SetVector3($"pointLights[{i}].position", pointLightPositions[i]);
                lightingShader.SetVector3($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                lightingShader.SetVector3($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                lightingShader.SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                lightingShader.SetFloat($"pointLights[{i}].constant", 1.0f);
                lightingShader.SetFloat($"pointLights[{i}].linear", 0.09f);
                lightingShader.SetFloat($"pointLights[{i}].quadratic", 0.032f);
            }

            // Spot light
            lightingShader.SetVector3("spotLight.position", camera.Position);
            lightingShader.SetVector3("spotLight.direction", camera.Front);
            lightingShader.SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
            lightingShader.SetVector3("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
            lightingShader.SetVector3("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
            lightingShader.SetFloat("spotLight.constant", 1.0f);
            lightingShader.SetFloat("spotLight.linear", 0.09f);
            lightingShader.SetFloat("spotLight.quadratic", 0.032f);
            lightingShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            lightingShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));

            // Draw the planets
            RenderPlanets();

            // Draw lights
            GL.BindVertexArray(vaoLamp);
            lampShader.Use();
            lampShader.SetMatrix4("view", camera.GetViewMatrix());
            lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            // Draw sun
            // GL.BindVertexArray(vaoSun);
            // sunShader.Use();
            // sunShader.SetMatrix4("view", camera.GetViewMatrix());
            // sunShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            // GL.ActiveTexture(TextureUnit.Texture0);
            // sunMap.Use(TextureUnit.Texture0);
            // Matrix4 sunModel = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f) * -10.0f);
            // sunShader.SetMatrix4("model", sunModel);
            // sun.Render();

            // Draw all the lights at their positions
            // for (int i = 0; i < pointLightPositions.Length; i++)
            // {
            //     Matrix4 lampMatrix = Matrix4.CreateScale(0.2f) * Matrix4.CreateTranslation(pointLightPositions[i]);
            //     lampShader.SetMatrix4("model", lampMatrix);
            //     GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            // }

            SwapBuffers();
            PrintFPS();
        }

        //Resize
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            camera.AspectRatio = Size.X / (float)Size.Y;
        }

        //Keyboard
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            // Toggle VSync when 'V' is pressed
            if (input.IsKeyDown(Keys.V) && timer.ElapsedMilliseconds > 500)
            {
                timer.Restart();
                vsyncEnabled = !vsyncEnabled;
                VSync = vsyncEnabled ? VSyncMode.On : VSyncMode.Off;
            }

            // enable/disable Fullscreen when F is pressed + debounce time of 500ms
            if (input.IsKeyDown(Keys.F) && timer.ElapsedMilliseconds > 500)
            {
                timer.Restart();
                WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
            }
            const float cameraSpeed = 3.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                camera.Position += camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                camera.Position -= camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                camera.Position -= camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                camera.Position += camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                camera.Position += camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                camera.Position -= camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = MouseState;

            if (firstMove) // This bool variable is initially set to true.
            {
                lastPos = new Vector2(mouse.X, mouse.Y);
                firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - lastPos.X;
                var deltaY = mouse.Y - lastPos.Y;
                lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                camera.Yaw += deltaX * sensitivity;
                camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }

        protected override void OnUnload()
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(vaoModel);

            //GL.DeleteProgram(shader.Handle);

            base.OnUnload();
        }

        // Mouse
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        // In the mouse wheel function, we manage all the zooming of the camera.
        // This is simply done by changing the FOV of the camera.
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            camera.Fov -= e.OffsetY;
        }

        //show all HW info in  console
        public static void ShowHWinfo()
        {
            // get OpenGL version
            string version = GL.GetString(StringName.Version);
            Console.WriteLine($"OpenGL version {version}");
            // get GLSL version
            string glslVersion = GL.GetString(StringName.ShadingLanguageVersion);
            Console.WriteLine($"GLSL version {glslVersion}");
            // get OpenGL vendor
            string vendor = GL.GetString(StringName.Vendor);
            Console.WriteLine($"OpenGL vendor {vendor}");
            // get OpenGL renderer
            string renderer = GL.GetString(StringName.Renderer);
            Console.WriteLine($"OpenGL renderer {renderer}");
            // get OpenGL extensions
            string extensions = GL.GetString(StringName.Extensions);
            Console.WriteLine($"OpenGL extensions {extensions}");
        }

        //print FPS in console
        public static void PrintFPS()
        {
            // FPS counter
            if (timer.ElapsedMilliseconds > 1000)
            {
                timer.Restart();
                Console.WriteLine($"FPS: {fps}");
                fps = 0;
            }
            fps++;
        }

        //set solar system
        public void SetSolarSystem()
        {
            // sizes of objects sun, mercury, venus, earth, moon, mars, jupiter, saturn, uran, neptun
            float[] solarRadius = [109.0f, 0.33f, 0.8f, 1.0f, 0.2f, 0.5f, 11.0f, 9.0f, 4.0f, 3.8f];
            //sun
            sun = new Sphere(radius: solarRadius[0], sectorCount: 36, stackCount: 18);
            sunMap = Texture.LoadFromFile("Resources/2k_sun.jpg");

            //mercury
            mercury = new Sphere(radius: solarRadius[1], sectorCount: 36, stackCount: 18);
            mercuryMap = Texture.LoadFromFile("Resources/2k_mercury.jpg");

            // venus
            venus = new Sphere(radius: solarRadius[2], sectorCount: 36, stackCount: 18);
            venusMap = Texture.LoadFromFile("Resources/2k_venus_surface.jpg");
            venusAtmosphere = Texture.LoadFromFile("Resources/2k_venus_atmosphere.jpg");

            // earth
            earth = new Sphere(radius: solarRadius[3], sectorCount: 36, stackCount: 18);
            earthMap = Texture.LoadFromFile("Resources/2k_earth_daymap.jpg");
            earthAtmosphere = Texture.LoadFromFile("Resources/2k_earth_clouds.jpg");

            // moon
            moon = new Sphere(radius: solarRadius[4], sectorCount: 36, stackCount: 18);
            moonMap = Texture.LoadFromFile("Resources/2k_moon.jpg");

            // mars
            mars = new Sphere(radius: solarRadius[5], sectorCount: 36, stackCount: 18);
            marsMap = Texture.LoadFromFile("Resources/2k_mars.jpg");

            // jupiter
            jupiter = new Sphere(radius: solarRadius[6], sectorCount: 36, stackCount: 18);
            jupiterMap = Texture.LoadFromFile("Resources/2k_jupiter.jpg");

            // saturn
            saturn = new Sphere(radius: solarRadius[7], sectorCount: 36, stackCount: 18);
            saturnMap = Texture.LoadFromFile("Resources/2k_saturn.jpg");

            // uranus
            uran = new Sphere(radius: solarRadius[8], sectorCount: 36, stackCount: 18);
            uranMap = Texture.LoadFromFile("Resources/2k_uranus.jpg");

            // neptune
            neptun = new Sphere(radius: solarRadius[9], sectorCount: 36, stackCount: 18);
            neptunMap = Texture.LoadFromFile("Resources/2k_neptune.jpg");

        }

        //render planets
        public void RenderPlanets()
        {
            // distance from the sun
            //                  sun, mercury, venus, earth, moon, mars, jupiter, saturn, uran, neptun
            float[] distance = [-10.0f, 0.39f, 0.72f, 1.0f, 1.1f, 1.52f, 5.20f, 9.54f, 19.22f, 30.06f];


            //Draw Sun
            GL.ActiveTexture(TextureUnit.Texture0);
            sunMap.Use(TextureUnit.Texture0);
            Matrix4 sunModel = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f) * distance[0]);
            lightingShader.SetMatrix4("model", sunModel);
            sun.Render();

            //Draw Mercury
            GL.ActiveTexture(TextureUnit.Texture0);
            mercuryMap.Use(TextureUnit.Texture0);
            Matrix4 mercuryModel = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f) * distance[1]);
            lightingShader.SetMatrix4("model", mercuryModel);
            mercury.Render();

            // Draw Venus
            GL.ActiveTexture(TextureUnit.Texture0);
            venusMap.Use(TextureUnit.Texture0);
            GL.ActiveTexture(TextureUnit.Texture1);
            venusAtmosphere.Use(TextureUnit.Texture1);
            Matrix4 venusModel = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f) * distance[2]);
            lightingShader.SetMatrix4("model", venusModel);
            venus.Render();

            // Draw the Earth
            GL.ActiveTexture(TextureUnit.Texture0);
            earthMap.Use(TextureUnit.Texture0);
            // earth atmosphere
            GL.ActiveTexture(TextureUnit.Texture1);
            earthAtmosphere.Use(TextureUnit.Texture1);
            Matrix4 earthModel = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f) * distance[3]);
            lightingShader.SetMatrix4("model", earthModel);
            earth.Render();

            // Draw the Moon
            GL.ActiveTexture(TextureUnit.Texture0);
            moonMap.Use(TextureUnit.Texture0);
            Matrix4 moonModel = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f) * distance[4]);
            lightingShader.SetMatrix4("model", moonModel);
            moon.Render();


            // Draw Mars
            GL.ActiveTexture(TextureUnit.Texture0);  // Activate texture unit 1
            marsMap.Use(TextureUnit.Texture0);
            Matrix4 marsModel = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f) * distance[5]);
            lightingShader.SetMatrix4("model", marsModel);
            mars.Render();

            // Draw Jupiter
            GL.ActiveTexture(TextureUnit.Texture0);  // Activate texture unit 1
            jupiterMap.Use(TextureUnit.Texture0);
            Matrix4 jupiterModel = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f) * distance[6]);
            lightingShader.SetMatrix4("model", jupiterModel);
            jupiter.Render();

            // Draw Saturn
            GL.ActiveTexture(TextureUnit.Texture0);  // Activate texture unit 1
            saturnMap.Use(TextureUnit.Texture0);
            Matrix4 saturnModel = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f) * distance[7]);
            lightingShader.SetMatrix4("model", saturnModel);
            saturn.Render();

            // Draw Uranus
            GL.ActiveTexture(TextureUnit.Texture0);  // Activate texture unit 1
            uranMap.Use(TextureUnit.Texture0);
            Matrix4 uranModel = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f) * distance[8]);
            lightingShader.SetMatrix4("model", uranModel);
            uran.Render();

            // Draw Neptune
            GL.ActiveTexture(TextureUnit.Texture0);  // Activate texture unit 1
            neptunMap.Use(TextureUnit.Texture0);
            Matrix4 neptunModel = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f) * distance[9]);
            lightingShader.SetMatrix4("model", neptunModel);
            neptun.Render();
        }
    }
}