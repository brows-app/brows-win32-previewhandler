namespace Brows.Win32.PlatformInvoke;

internal struct LOGFONTW {
    public int lfHeight;
    public int lfWidth;
    public int lfEscapement;
    public int lfOrientation;
    public int lfWeight;
    public byte lfItalic;
    public byte lfUnderline;
    public byte lfStrikeOut;
    public byte lfCharSet;
    public byte lfOutPrecision;
    public byte lfClipPrecision;
    public byte lfQuality;
    public byte lfPitchAndFamily;

    char lfFaceName0;
    char lfFaceName1;
    char lfFaceName2;
    char lfFaceName3;
    char lfFaceName4;
    char lfFaceName5;
    char lfFaceName6;
    char lfFaceName7;
    char lfFaceName8;
    char lfFaceName9;
    char lfFaceName10;
    char lfFaceName11;
    char lfFaceName12;
    char lfFaceName13;
    char lfFaceName14;
    char lfFaceName15;
    char lfFaceName16;
    char lfFaceName17;
    char lfFaceName18;
    char lfFaceName19;
    char lfFaceName20;
    char lfFaceName21;
    char lfFaceName22;
    char lfFaceName23;
    char lfFaceName24;
    char lfFaceName25;
    char lfFaceName26;
    char lfFaceName27;
    char lfFaceName28;
    char lfFaceName29;
    char lfFaceName30;
    char lfFaceName31;

    public string lfFaceName {
        get {
            char[] charArray = [
                lfFaceName0, lfFaceName1, lfFaceName2, lfFaceName3, lfFaceName4,
                lfFaceName5, lfFaceName6, lfFaceName7, lfFaceName8, lfFaceName9,
                lfFaceName10, lfFaceName11, lfFaceName12, lfFaceName13, lfFaceName14,
                lfFaceName15, lfFaceName16, lfFaceName17, lfFaceName18, lfFaceName19,
                lfFaceName20, lfFaceName21, lfFaceName22, lfFaceName23, lfFaceName24,
                lfFaceName25, lfFaceName26, lfFaceName27, lfFaceName28, lfFaceName29,
                lfFaceName30, lfFaceName31];
            int length;
            for (length = 0; length < charArray.Length; ++length)
                if (charArray[length] == '\0')
                    break;
            if (length == charArray.Length)
                return null;
            return new string(charArray, 0, length);
        }
        set {
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }
            int length = value.Length;
            if (length > 31)
                throw new ArgumentOutOfRangeException("value", "The string is too long.");
            char CharacterAt(int index) => length > index ? value[index] : '\0';
            lfFaceName0 = CharacterAt(0);
            lfFaceName1 = CharacterAt(1);
            lfFaceName2 = CharacterAt(2);
            lfFaceName3 = CharacterAt(3);
            lfFaceName4 = CharacterAt(4);
            lfFaceName5 = CharacterAt(5);
            lfFaceName6 = CharacterAt(6);
            lfFaceName7 = CharacterAt(7);
            lfFaceName8 = CharacterAt(8);
            lfFaceName9 = CharacterAt(9);
            lfFaceName10 = CharacterAt(10);
            lfFaceName11 = CharacterAt(11);
            lfFaceName12 = CharacterAt(12);
            lfFaceName13 = CharacterAt(13);
            lfFaceName14 = CharacterAt(14);
            lfFaceName15 = CharacterAt(15);
            lfFaceName16 = CharacterAt(16);
            lfFaceName17 = CharacterAt(17);
            lfFaceName18 = CharacterAt(18);
            lfFaceName19 = CharacterAt(19);
            lfFaceName20 = CharacterAt(20);
            lfFaceName21 = CharacterAt(21);
            lfFaceName22 = CharacterAt(22);
            lfFaceName23 = CharacterAt(23);
            lfFaceName24 = CharacterAt(24);
            lfFaceName25 = CharacterAt(25);
            lfFaceName26 = CharacterAt(26);
            lfFaceName27 = CharacterAt(27);
            lfFaceName28 = CharacterAt(28);
            lfFaceName29 = CharacterAt(29);
            lfFaceName30 = CharacterAt(30);
            lfFaceName31 = '\0';
        }
    }
}
