namespace Common
{
    public enum BoidMatesAlgorithm
    {
        Bruteforce,
        KD_Tree_Bounds, //bounds/aka interval query
        UnityOctreeBounds,//Also Bounds
        SpatialHashing
    }
}