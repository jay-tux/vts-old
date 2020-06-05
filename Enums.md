# VTS - Jay.VTS.Enums
Contains only one enum at the moment, but should actually contain more.
 - ``[Flags] public enum ElementType``;  
 Defines the possible types for a ``LineElement``. This is a flags enum (uses only powers of two) ranging from 1 (``1 << 0``) to 16384 (``1 << 14``). Options are, in order: ``Block, None, Void, Preproc, Class, Action, Control, Return, Member, Field, Separator, Identifier, Literal, Operator, Comment``.
