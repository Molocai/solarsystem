namespace UbiSolarSystem
{
    /// <summary>
    /// Describes the user defined collision layers
    /// </summary>
    public enum LAYER
    {
        FLOOR = 8, // "Floor"
        PLANET = 9 // "Planet"
    }

    public static class LayerManager
    {
        public static int ToInt(LAYER layer)
        {
            return (int)layer;
        }
    }

    /* I should probably double check if LayerMask.NameToLayer returns something
     * equivalent to the enum's value but for this project we only have two layers
     * so it's not very important
     */
}
