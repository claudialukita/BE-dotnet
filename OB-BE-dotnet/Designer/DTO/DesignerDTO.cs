using OB_BE_dotnet.Dress.DTO;
using System;
using System.Collections.Generic;

namespace OB_BE_dotnet.Designer.DTO
{
    public class DesignerDTO : DesignerWithoutIdDTO
    {
        public Guid Id { get; set; }

    }

    public class DesignerWithoutIdDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class DesignerDressesDTO : DesignerDTO
    {
        public List<DressDTO> Dresses { get; set; }
    }
}
