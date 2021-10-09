using Eleon.Modding;

namespace GalacticWaez
{
    public static class VectorExtensions
    {
        public static VectorInt3 Divide(this VectorInt3 vector, int divisor)
            => new VectorInt3(
                vector.x / divisor,
                vector.y / divisor,
                vector.z / divisor);

        public static VectorInt3 Multiply(this VectorInt3 vector, int multiplier)
            => new VectorInt3(
                vector.x * multiplier,
                vector.y * multiplier,
                vector.z * multiplier);
    }
}