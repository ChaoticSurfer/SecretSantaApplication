using System.Collections.Generic;

namespace SecretSantaApplication.Models
{
    public static class RoomRepository
    {
        public static List<Room> _gameRooms = new List<Room>()
        {
            new Room() {Creator = "anri", Description = "btu uni shekrebai1", LogoName = "logoi1", Name = "BTU Gang1"},
            new Room() {Creator = "anri2", Description = "btu uni shekrebai2", LogoName = "logoi2", Name = "BTU Gang2"},
            new Room() {Creator = "anri3", Description = "btu uni shekrebai3", LogoName = "logoi3", Name = "BTU Gang3"}
        };

        public static IEnumerable<Room> Rooms => _gameRooms;
    }
}