using ConventionalBoid;

namespace HasseDiagram
{
    public class NodeBoid : Boid
    {
        public int SetValue; //Represents a Value which can be compared to others, can be made Generic
       //For lattice diagram drawing
        public void HardTranslation()
        {
            transform.position += velocity;
        }
    }
}