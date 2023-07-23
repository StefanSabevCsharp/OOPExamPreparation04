using BookingApp.Core.Contracts;
using BookingApp.Models.Hotels;
using BookingApp.Models.Hotels.Contacts;
using BookingApp.Models.Rooms;
using BookingApp.Models.Rooms.Contracts;
using BookingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Core
{
    public class Controller : IController
    {
        private HotelRepository hotels;
        public Controller()
        {
            hotels = new HotelRepository();
        }
        public string AddHotel(string hotelName, int category)
        {
           IHotel hotel = hotels.All().FirstOrDefault(h => h.FullName == hotelName);
            if (hotel != null)
            {
               return $"{string.Format(Utilities.Messages.OutputMessages.HotelAlreadyRegistered,hotelName)}";
            }
            IHotel newHotel = new Hotel(hotelName, category);
            hotels.AddNew(newHotel);
            return $"{string.Format(Utilities.Messages.OutputMessages.HotelSuccessfullyRegistered,category, hotelName)}";

        }

        public string UploadRoomTypes(string hotelName, string roomTypeName)
        {
            IHotel hotel = hotels.All().FirstOrDefault(h => h.FullName == hotelName);
            if (hotel == null)
            {
                return $"{string.Format(Utilities.Messages.OutputMessages.HotelNameInvalid, hotelName)}";
            }
            if (hotel.Rooms.All().FirstOrDefault(r => r.GetType().Name == roomTypeName) != null)
            {
                return $"{string.Format(Utilities.Messages.OutputMessages.RoomTypeAlreadyCreated)}";
            }
          
            if (roomTypeName != "Studio" && roomTypeName != "DoubleBed" && roomTypeName != "Apartment")
            {
                throw new ArgumentException(Utilities.Messages.ExceptionMessages.RoomTypeIncorrect);
            }
            switch (roomTypeName)
            {
                case"Studio":
                    IRoom studio = new Studio();
                    hotel.Rooms.AddNew(studio);
                    break;
                    case "DoubleBed":
                    IRoom doubleBed = new DoubleBed();
                    hotel.Rooms.AddNew(doubleBed);
                    break;
                    case "Apartment":
                    IRoom apartment = new Apartment();
                    hotel.Rooms.AddNew(apartment);
                    break;
            }

            return $"{string.Format(Utilities.Messages.OutputMessages.RoomTypeAdded,roomTypeName,hotelName)}";

            
        }

        public string SetRoomPrices(string hotelName, string roomTypeName, double price)
        {
            IHotel hotel = hotels.All().FirstOrDefault(h => h.FullName == hotelName);
            if (hotel == null)
            {
                return $"{string.Format(Utilities.Messages.OutputMessages.HotelNameInvalid, hotelName)}";
            }
            
            if (roomTypeName != "Studio" && roomTypeName != "DoubleBed" && roomTypeName != "Apartment")
            {
                throw new ArgumentException(Utilities.Messages.ExceptionMessages.RoomTypeIncorrect);
            }
            //•	If the given type is not created yet, returns: "Room type is not created yet!"
            //var a = hotel.Rooms.All().FirstOrDefault(r => r.GetType().Name == roomTypeName);

            //if (hotel.Rooms.All().FirstOrDefault(r => r.GetType().Name == roomTypeName) == null)
            //{
            //    return $"{string.Format(Utilities.Messages.OutputMessages.RoomTypeNotCreated)}";
            //}
            IRoom room = hotel.Rooms.All().FirstOrDefault(r => r.GetType().Name == roomTypeName);
            if(room.PricePerNight != 0)
            {
                throw new InvalidOperationException(Utilities.Messages.ExceptionMessages.PriceAlreadySet);
            }
            room.SetPrice(price);

            return $"{string.Format(Utilities.Messages.OutputMessages.PriceSetSuccessfully, roomTypeName,hotelName)}";
        }

        public string BookAvailableRoom(int adults, int children, int duration, int category)
        {
            throw new NotImplementedException();
        }

        public string HotelReport(string hotelName)
        {
            throw new NotImplementedException();
        }

        

       
    }
}
