// ReSharper disable InconsistentNaming
namespace DS1Counter.Savegame
{
    public class SaveGameOffsets
    {
        public const long SLOT_BEGIN = 0x2c0;
        public const long SLOT_LENGTH = 0x60020;

        public const long CHARACTER_NAME = 0x100;
        public const long CHARACTER_DEATHS = 0x1f128;

        public const long CHARACTER_Vitality = 0x98;        //8 bytes
        public const long CHARACTER_Attunement = 0xa0;      //8 bytes
        public const long CHARACTER_Endurance = 0xa8;       //8 bytes
        public const long CHARACTER_Strength = 0xb0;        //8 bytes
        public const long CHARACTER_Dexterity = 0xb8;       //8 bytes
        public const long CHARACTER_Intelligence = 0xc0;    //8 bytes
        public const long CHARACTER_Faith = 0xc8;           //8 bytes
        public const long CHARACTER_Humanity = 0xd8;        //8 bytes
        public const long CHARACTER_Resistance = 0xe0;      //8 bytes
        public const long CHARACTER_Level = 0xe8;           //4 bytes
        public const long CHARACTER_Souls = 0xec;           //4 bytes
        public const long CHARACTER_Earned_Souls = 0xf0;    //8 bytes

        public const long CHARACTER_Male = 0x122;           //1 byte
        public const long CHARACTER_Class = 0x126;          //1 byte

        public const long CHARACTER_Left_arrows_slot = 0x228;    //4 bytes
        public const long CHARACTER_Left_bolts_slot = 0x22c;    //4 bytes
        public const long CHARACTER_Right_arrows_slot = 0x230;    //4 bytes
        public const long CHARACTER_Right_bolts_slot = 0x234;    //4 bytes
        public const long CHARACTER_Left_ring_slot = 0x24c;    //4 bytes
        public const long CHARACTER_Right_ring_slot = 0x250;    //4 bytes
        public const long CHARACTER_First_quick_slot = 0x254;    //4 bytes
        public const long CHARACTER_Second_quick_slot = 0x258;    //4 bytes
        public const long CHARACTER_Third_quick_slot = 0x25c;    //4 bytes
        public const long CHARACTER_Fourth_quick_slot = 0x260;    //4 bytes
        public const long CHARACTER_Fifth_quick_slot = 0x264;    //4 bytes
        public const long CHARACTER_Head_wearing_slot = 0x2a4;    //4 bytes
        public const long CHARACTER_Body_wearing_slot = 0x2a8;    //4 bytes
        public const long CHARACTER_Hands_wearing_slot = 0x2ac;    //4 bytes
        public const long CHARACTER_Legs_wearing_slot = 0x2b0;    //4 bytes
    }
}
