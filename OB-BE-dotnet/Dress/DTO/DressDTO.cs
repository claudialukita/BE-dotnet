using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OB_BE_dotnet.Dress.DTO
{
    public class DressDTO : DressWithoutIdDTO
    {
        public Guid? Id { get; set; }

        public static implicit operator DressDTO(DressModel v)
        {
            throw new NotImplementedException();
        }
    }

    public class DressWithoutIdDTO
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }
    }
}
