using MyCity.DataAccess;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyCity.DataModel.AppModels {
	public interface ITargetAreaService : IDataService<TargetArea> {
		TargetArea GetClosestArea(double lat, double lng);
		double GetDistance(double baseLat, double baseLng, double lat, double lng);

	}

	public class TargetAreaService : DataService<TargetArea>, ITargetAreaService {
		public TargetAreaService(IMyUnitOfWork uow) : base(uow) {
		}

		public TargetArea GetClosestArea(double lat, double lng) {
			var gravities = this.GetAll(x => true).ToList();

			if (gravities.Count == 0)
				return null;

			var firstGravity = gravities.First();
			double exDistance = GetDistance(firstGravity.Latlatitude, firstGravity.Longitude, lat, lng);

			TargetArea bestGravity = null;

			foreach (var gravity in gravities) {
				var current = GetDistance(gravity.Latlatitude, gravity.Longitude, lat, lng);

				if (current <= exDistance)
					bestGravity = gravity;

				exDistance = current;
			}

			return bestGravity;
		}

		public double GetDistance(double baseLat, double baseLng, double lat, double lng) {
			var distanceX = lat - baseLat;
			var distanceY = lng - baseLng;

			if (distanceX < 0)
				distanceX = (-1 * distanceX);

			if (distanceY < 0)
				distanceY = (-1 * distanceY);

			var distance = ((distanceX * distanceX) + (distanceY * distanceY));
			return Math.Sqrt(distance);
		}

	}
}