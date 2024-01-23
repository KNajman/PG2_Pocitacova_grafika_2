using OpenTK.Graphics.OpenGL4;

namespace Final
{
    public class Sphere
    {
        private readonly int VAO, VBO, EBO;
        private readonly int vertexCount;

        public Sphere(float radius, int sectorCount, int stackCount)
        {
            List<float> vertices = [];
            List<int> indices = [];

            float sectorStep = 2 * MathF.PI / sectorCount;
            float stackStep = MathF.PI / stackCount;
            float sectorAngle, stackAngle;

            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngle = MathF.PI / 2 - i * stackStep;  // starting from pi/2 to -pi/2
                float xy = radius * MathF.Cos(stackAngle);  // r * cos(u)
                float z = radius * MathF.Sin(stackAngle);   // r * sin(u)

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;  // starting from 0 to 2pi

                    // vertex position
                    float x = xy * MathF.Cos(sectorAngle);  // r * cos(u) * cos(v)
                    float y = xy * MathF.Sin(sectorAngle);  // r * cos(u) * sin(v)
                    float vz = radius * MathF.Sin(stackAngle); // r * sin(u)

                    // normalized vertex normal
                    float nx = x / radius;
                    float ny = y / radius;
                    float nz = vz / radius;

                    // vertex texture coordinates
                    float s = (float)j / sectorCount;
                    float t = (float)i / stackCount;

                    // add vertex data to the lists
                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(vz);
                    vertices.Add(nx);
                    vertices.Add(ny);
                    vertices.Add(nz);
                    vertices.Add(s);
                    vertices.Add(t);

                    if (i != stackCount && j != sectorCount)
                    {
                        // indices
                        int currentRow = i;
                        int nextRow = i + 1;
                        int nextSect = j + 1;

                        indices.Add(currentRow * (sectorCount + 1) + j);
                        indices.Add(nextRow * (sectorCount + 1) + j);
                        indices.Add(nextRow * (sectorCount + 1) + nextSect);

                        indices.Add(currentRow * (sectorCount + 1) + j);
                        indices.Add(nextRow * (sectorCount + 1) + nextSect);
                        indices.Add(currentRow * (sectorCount + 1) + nextSect);
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
