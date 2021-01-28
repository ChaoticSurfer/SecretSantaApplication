using System.Collections;
using System.Collections.Generic;
using SecretSantaApplication.Models;

namespace SecretSantaApplication.Views.ViewModels
{
    public class RoomsUserToRoomsViewModel
    {
        public RoomsUserToRoomsViewModel(){}

        public RoomsUserToRoomsViewModel(IEnumerable<Room> _rooms, IEnumerable<UserToRoom> _userToRooms)
        {
            Rooms = _rooms;
            UserToRooms = _userToRooms;
        }

         public IEnumerable<Room> Rooms { get; private set; }
         public IEnumerable<UserToRoom> UserToRooms { get; private set; }
    }
}