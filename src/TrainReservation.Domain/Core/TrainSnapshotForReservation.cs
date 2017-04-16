using System;
using System.Collections.Generic;
using System.Linq;
using Value;

namespace TrainReservation.Domain.Core
{
    /// <summary>
    /// Aggregate allowing seat allocation during reservations following the business rules defined with the domain experts. 
    /// </summary>
    public class TrainSnapshotForReservation : ValueType<TrainSnapshotForReservation>
    {
        private const double SeventyPercent = 0.70d;
        private readonly Dictionary<string, Coach> coaches;
        private readonly List<SeatWithBookingReference> seatsWithBookingReferences;

        public string TrainId { get; }

        public TrainSnapshotForReservation(string trainId, IEnumerable<SeatWithBookingReference> seatsWithBookingReferences)
        {
            TrainId = trainId;

            this.seatsWithBookingReferences = new List<SeatWithBookingReference>(seatsWithBookingReferences);

            coaches = CoachFactory.InstantiateCoaches(trainId, seatsWithBookingReferences);
        }

        public int OverallTrainCapacity => coaches.Values.Sum(coach => coach.OverallCoachCapacity);

        public int MaxReservableSeatsFollowingThePolicy => (int) Math.Round(OverallTrainCapacity * SeventyPercent);

        public int AlreadyReservedSeatsCount { get { return coaches.Values.Sum(coach => coach.AlreadyReservedSeatsCount); } }

        public IEnumerable<SeatWithBookingReference> SeatsWithBookingReferences => seatsWithBookingReferences;

        public int CoachCount => coaches.Count;

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new object[] {TrainId, new ListByValue<SeatWithBookingReference>(seatsWithBookingReferences)};
        }

        public ReservationOption Reserve(int requestedSeatCount)
        {
            var option = new ReservationOption(TrainId, requestedSeatCount);

            if (AlreadyReservedSeatsCount + requestedSeatCount > MaxReservableSeatsFollowingThePolicy)
            {
                return option;
            }

            // Try the nice way (i.e. by respecting the "no more 70% of every coach" rule)
            foreach (var coach in coaches.Values)
            {
                if (coach.HasEnoughAvailableSeatsIfWeFollowTheIdealPolicy(requestedSeatCount))
                {
                    option = coach.Reserve(requestedSeatCount);
                    if (option.IsFullfiled)
                    {
                        return option;
                    }
                }
            }

            if (!option.IsFullfiled)
            {
                // Try the hard way (i.e. don't respect the "no more 70% of every coach" rule)
                foreach (var coach in coaches.Values)
                {
                    option = coach.Reserve(requestedSeatCount);
                    if (option.IsFullfiled)
                    {
                        return option;
                    }
                }
            }

            return option;
        }
    }
}