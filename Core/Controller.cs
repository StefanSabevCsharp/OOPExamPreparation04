using BookingApp.Core.Contracts;
using BookingApp.Models.Bookings;
using BookingApp.Models.Bookings.Contracts;
using BookingApp.Models.Hotels;
using BookingApp.Models.Hotels.Contacts;
using BookingApp.Models.Rooms;
using BookingApp.Models.Rooms.Contracts;
using BookingApp.Repositories;
using BookingApp.Utilities.Messages;
using System;
using System.Linq;
using System.Text;

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
                return $"{string.Format(Utilities.Messages.OutputMessages.HotelAlreadyRegistered, hotelName)}";
            }
            IHotel newHotel = new Hotel(hotelName, category);
            hotels.AddNew(newHotel);
            return $"{string.Format(Utilities.Messages.OutputMessages.HotelSuccessfullyRegistered, category, hotelName)}";

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

            //switch (roomTypeName)
            //{
            //    case"Studio":
            //        IRoom studio = new Studio();
            //        hotel.Rooms.AddNew(studio);
            //        break;
            //        case "DoubleBed":
            //        IRoom doubleBed = new DoubleBed();
            //        hotel.Rooms.AddNew(doubleBed);
            //        break;
            //        case "Apartment":
            //        IRoom apartment = new Apartment();
            //        hotel.Rooms.AddNew(apartment);
            //        break;
            //}

            IRoom room = (IRoom)Activator.CreateInstance(Type.GetType($"BookingApp.Models.Rooms.{roomTypeName}"));
            hotel.Rooms.AddNew(room);

            return $"{string.Format(Utilities.Messages.OutputMessages.RoomTypeAdded, roomTypeName, hotelName)}";


        }

        public string SetRoomPrices(string hotelName, string roomTypeName, double price)
        {
            IHotel hotel = hotels.All().FirstOrDefault(h => h.FullName == hotelName);
            if (hotel == null)
            {
                return $"{string.Format(Utilities.Messages.OutputMessages.HotelNameInvalid, hotelName)}";
            }

            if (roomTypeName != nameof(Studio) && roomTypeName != nameof(DoubleBed) && roomTypeName != nameof(Apartment))
            {
                throw new ArgumentException(Utilities.Messages.ExceptionMessages.RoomTypeIncorrect);
            }

            IRoom room = hotel.Rooms.Select(roomTypeName);
            if (room == null)
            {
                return $"{string.Format(OutputMessages.RoomTypeNotCreated)}";
            }
            if (room.PricePerNight != 0)
            {
                throw new InvalidOperationException(Utilities.Messages.ExceptionMessages.PriceAlreadySet);
            }
            room.SetPrice(price);

            return $"{string.Format(Utilities.Messages.OutputMessages.PriceSetSuccessfully, roomTypeName, hotelName)}";
        }

        public string BookAvailableRoom(int adults, int children, int duration, int category)
        {

            IOrderedEnumerable<IHotel> hotelsCollection = hotels.All()
            .Where(h => h.Category == category)
            .OrderBy(h => h.FullName);

            if (hotelsCollection == null)
            {
                return $"{string.Format(OutputMessages.CategoryInvalid, category)}";
            }

            foreach (var hotel in hotelsCollection)
            {
                IRoom room = hotel.Rooms
                    .All()
                    .Where(x => x.PricePerNight > 0)
                    .OrderBy(x => x.BedCapacity)
                    .FirstOrDefault(x => x.BedCapacity >= adults + children);
                if (room != null)
                {
                    int totalBookings = hotel.Bookings.All().Count();
                    IBooking booking = new Booking(room, duration, adults, children, totalBookings + 1);
                    hotel.Bookings.AddNew(booking);
                    string hotelname = hotel.FullName;
                    return $"{string.Format(OutputMessages.BookingSuccessful, totalBookings + 1, hotelname)}";
                }

            }
            
            return $"{string.Format(OutputMessages.RoomNotAppropriate)}";
        }

        public string HotelReport(string hotelName)
        {
            IHotel hotel = hotels.All().FirstOrDefault(h => h.FullName == hotelName);
            if (hotel == null)
            {
                return $"{string.Format(OutputMessages.HotelNameInvalid, hotelName)}";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Hotel name: {hotelName}");
            sb.AppendLine($"--{hotel.Category} star hotel");
            sb.AppendLine($"--Turnover: {hotel.Turnover:F2} $");
            sb.AppendLine($"Bookings:");
            sb.AppendLine();

            if (hotel.Bookings.All().Count == 0)
            {
                sb.AppendLine("none");
            }
            else
            {
                foreach (var booking in hotel.Bookings.All())
                {
                    sb.AppendLine(booking.BookingSummary());
                    sb.AppendLine();
                }
            }
            return sb.ToString().TrimEnd();

        }




    }
}
