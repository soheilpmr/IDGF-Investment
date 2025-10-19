namespace IDGF.Core.Controllers.Dtos
{
    public class BondsGetDto
    {
        //public int ypeID { get; set; }
        public decimal Id { get; set; }
        public string Symbol { get; set; }
        public DateOnly MaturityDate { get; set; }
        public decimal FaceValue { get; set; }

        //public BondsGetDto GetDto(Bonds bonds)
        //{
        //    BondsTypeGetDto bondsTypeDto = new BondsTypeGetDto();
        //}
    }
}
