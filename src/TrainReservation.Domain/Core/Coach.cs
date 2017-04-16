using System;
using System.Collections.Generic;

namespace TrainReservation.Domain.Core
{
    public class Coach
    {
        private const double SeventyPercent = 0.70d;

        private readonly string trainId;
        public string CoachId { get; }
        public List<SeatWithBookingReference> Seats { get; }

        public Coach(string trainId, string coachId, List<SeatWithBookingReference> seats)
        {
            this.trainId = trainId;
            this.CoachId = coachId;
            this.Seats = seats;
        }


        #region Traits in common with TrainSnapshotForReservation?

        public int OverallCoachCapacity => this.Seats.Count;

        public int MaxReservableSeatsFollowingThePolicy => (int)Math.Round(OverallCoachCapacity * SeventyPercent);

        public int AlreadyReservedSeatsCount
        {
            get
            {
                var alreadyReservedSeatsCount = 0;
                // TODO: Linq this
                foreach (var seatWithBookingReference in Seats)
                {
                    if (!seatWithBookingReference.IsAvailable)
                    {
                        alreadyReservedSeatsCount++;
                    }
                }

                return alreadyReservedSeatsCount;
            }
        }

        #endregion

        public bool HasEnoughAvailableSeatsIfWeFollowTheIdealPolicy(int requestedSeatCount)
        {
            return (AlreadyReservedSeatsCount + requestedSeatCount) <= MaxReservableSeatsFollowingThePolicy;
        }

        public ReservationOption Reserve(int requestedSeatCount)
        {
            //foreach (var seat in SeatsWithBookingReferences)
            //{
            //    if (seat.IsAvailable)
            //    {
            //        option.AddSeatReservation(seat.Seat);
            //        if (option.IsFullfiled)
            //        {
            //            break;
            //        }
            //    }
            //}
            throw new System.NotImplementedException();
        }
    }
}