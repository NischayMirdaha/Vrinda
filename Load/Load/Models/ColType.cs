namespace Load.Models;

public enum ColType
{
    Text,           // str → TEXT
    Integer,        // float like 8.0 → INTEGER (cast to int)
    BigInt,         // float like 476973091.0 → BIGINT
    Double,         // float like 26.9466959 → DOUBLE PRECISION
    SmallInt,       // float 0.0/1.0 → SMALLINT (boolean-like)
    Timestamp,      // str "2025-07-03T18:37:30.626" → TIMESTAMPTZ
    Ignore          // consent text, section headers — skip
}