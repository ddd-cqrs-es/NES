﻿namespace NES.NEventStore.Raven
{
    public static class CustomizedRavenSerializerWireupExtensions
    {
        public static CustomizedRavenSerializerWireup UsingCustomizedRavenSerializer(this NESWireup wireup)
        {
            return new CustomizedRavenSerializerWireup(wireup);
        }
    }
}