using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Questions.Must_Revise_Implementatiion
{
    public interface ICar
    {
        public void Start();
    }

    public class SixSeaterCar : ICar
    {
        public void Start() { Console.WriteLine("SixSeaterCar created!"); }
    }

    public class FourSeaterCar : ICar
    {
        public void Start() { Console.WriteLine("FourSeaterCar created!"); }
    }

    public class CarFactory
    {
        public ICar GetCar(string carType)
        {
            switch (carType)
            {
                case "SixSeaterCar":
                    return new SixSeaterCar();
                case "FourSeaterCar":
                    return new FourSeaterCar();
                default: throw new ArgumentException("Unknown car type");
            }
        }
    }

    public class CallFactoryDemo
    {
        public void RunFactoryDemo()
        {
            CarFactory carFactory = new CarFactory();
            ICar sixSeaterCar = carFactory.GetCar("SixSeaterCar");
            sixSeaterCar.Start();

            ICar fourSeaterCar = carFactory.GetCar("FourSeaterCar");
            fourSeaterCar.Start();
        }
    }
}
