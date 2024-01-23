using OpenTK.Graphics.OpenGL4;

namespace Final
{
    public class Sphere
    {
        private int VAO, VBO, EBO;
        private int vertexCount;

        public Sphere(float radius, int sectorCount, int stackCount)
        {
            List<float> vertices = new List<float>();
            List<int> indices = new List<int>();

            float x, y, z, xy;  // vertex position
            float nx, ny, nz, lengthInv = 1.0f / radius;  // vertex normal
            float s, t;  // vertex texture coordinates

            float sectorStep = 2 * MathF.PI / sectorCount;
            float stackStep = MathF.PI / stackCount;
            float sectorAngle, stackAngle;

            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngle = MathF.PI / 2 - i * stackStep;  // starting from pi/2 to -pi/2
                xy = radius * MathF.Cos(stackAngle);  // r * cos(u)
                z = radius * MathF.Sin(stackAngle);   // r * sin(u)

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;  // starting from 0 to 2pi

                    // vertex position
                    x = xy * MathF.Cos(sectorAngle);  // r * cos(u) * cos(v)
                    y = xy * MathF.Sin(sectorAngle);  // r * cos(u) * sin(v)
                    z = radius * MathF.Sin(stackAngle); // r * sin(u)

                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(z);

                    // normalized vertex normal
                    nx = x * lengthInv;
                    ny = y * lengthInv;
                    nz = z * lengthInv;
                    vertices.Add(nx);
                    vertices.Add(ny);
                    vertices.Add(nz);

                    // vertex texture coordinates
                    s = (float)j / sectorCount;
                    t = (float)i / stackCount;
                    vertices.Add(s);
                    vertices.Add(t);

                    if (i != stackCount)
                    {
                        // indices
                        int currentRow = i;
                        int nextRow = i + 1;
                        int nextSect = (j + 1) % sectorCount;

                        indices.Add(currentRow * (sectorCount + 1) + j);
                        indices.Add(nextRow * (sectorCount + 1) + j);
                        indices.Add(nextRow * (sectorCount + 1) + nextSect);

                        indices.Add(currentRow * (sectorCount + 1) + j);
                        indices.Add(nextRow * (sectorCount + 1) + nextSect);
                        indices.Add(currentRow * (sectorCount + 1) + (j + 1) % sectorCount);
                    }
                }
            }

            vertexCount = indices.Count;

            // Create and bind VAO, VBO, and EBO
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);

            // Set vertex attribute pointers
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            // Unbind VAO
            GL.BindVertexArray(0);
        }

        public void Render()
        {
            // Bind VAO and draw
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, vertexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
    }
