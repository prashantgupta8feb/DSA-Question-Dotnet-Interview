using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Questions.Must_Revise_Implementatiion
{
    public interface ICar2 { }

    public interface IBike { }

    public class TataCar : ICar2
    {
        public void Manufacturer() { }
    }

    public class TataBike : IBike
    {
        public void Manufacturer() { }
    }


    public class TeslaCar : ICar2
    {
        public void Manufacturer() { }
    }

    public class TeslaBike : IBike
    {
        public void Manufacturer() { }
    }

    public abstract class VehicleCompany
    {
        public abstract ICar2 GetCar();

        public abstract IBike GetBike();
    }

    public class TataCompany : VehicleCompany
    {
        public override ICar2 GetCar()
        {
            Console.WriteLine("Tata Car Created");
            return new TataCar();
        }

        public override IBike GetBike()
        {
            Console.WriteLine("Tata Bike Created");
            return new TataBike();
        }
    }

    public class TeslaCompany : VehicleCompany
    {
        public override ICar2 GetCar()
        {
            Console.WriteLine("Tesla Car Created");
            return new TeslaCar();
        }

        public override IBike GetBike()
        {
            Console.WriteLine("Tesla Bike Created");
            return new TeslaBike();
        }
    }

    public class CallAbstractFactoryDemo
    {
        public void RunAbstractFactoryDemo()
        {
            VehicleCompany teslaCarCompany = new TeslaCompany();
            IBike teslaBike = teslaCarCompany.GetBike();
            ICar2 teslaCar = teslaCarCompany.GetCar();

            VehicleCompany tataCarCompany = new TataCompany();
            IBike tataBike = tataCarCompany.GetBike();
            ICar2 tataCar = tataCarCompany.GetCar();
        }
    }
}
