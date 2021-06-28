using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * CREATE [UNIQUE | FULLTEXT | SPATIAL] INDEX index_name
    [index_type]
    ON tbl_name (key_part,...)
    [index_option]
    [algorithm_option | lock_option] ...

key_part: {col_name [(length)] | (expr)} [ASC | DESC]

index_option: {
    KEY_BLOCK_SIZE [=] value
  | index_type
  | WITH PARSER parser_name
  | COMMENT 'string'
  | {VISIBLE | INVISIBLE}
  | ENGINE_ATTRIBUTE [=] 'string'
  | SECONDARY_ENGINE_ATTRIBUTE [=] 'string'
}

index_type:
    USING {BTREE | HASH}

algorithm_option:
    ALGORITHM [=] {DEFAULT | INPLACE | COPY}

lock_option:
    LOCK [=] {DEFAULT | NONE | SHARED | EXCLUSIVE}
*/

public enum IndexType {
    BTREE,
    HASH
}

public enum AlgorithmOption {
    DEFAULT , INPLACE , COPY
}

namespace prestoMySQL.Column.Attribute {

    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false , Inherited = false )]
    public sealed class DDIndexAttribute : System.Attribute {

        public string Name;

        public IndexType? IndexType = null;

    }

}
