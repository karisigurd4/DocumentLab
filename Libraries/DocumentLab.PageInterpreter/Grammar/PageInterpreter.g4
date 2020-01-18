grammar PageInterpreter;

/*
 * Parser Rules
 */

compileUnit
	: query+ EOF
	;

query
	: ( Text Colon )? ( pattern )+ 
	;

pattern
	: Any? subset? ( capture | traverse | textType | rightDownSearch )* SemiColon
	;

subset
	: Subset Parameters
	;

subsetPart
	: Part=Top
	| Part=Bottom
	| Part=Left
	| Part=Right
	;

rightDownSearch
	: RightDown Steps=Numbers
	;

table
	: capture+
	;

capture
	: propertyName? LBracket ( textType ) RBracket
	;

traverse
	: Direction=Up
	| Direction=Down
	| Direction=Left
	| Direction=Right
	;

propertyName
	: SingleQuote Text SingleQuote Colon
	;

textType
	: Text textTypeParameters ( Or Text textTypeParameters )*
	;

textTypeParameters
	: Parameters?
	;

/*
 * Lexer Rules
 */

Any: 'Any';
RightDown: 'RD';

Or: '||';
RBracket: ']';
LBracket: '[';
SemiColon: ';';
Colon: ':';
SingleQuote: '\'';
Comma: ',';

Subset: 'Subset';
Top: 'Top';
Bottom: 'Bottom';
Up: 'Up';
Down: 'Down';
Left: 'Left';
Right: 'Right';

Parameters: LParenthesis .*? RParenthesis;

Numbers: [0-9]+;
Text: [A-Za-z0-9]+;

LineComment: '//' ~[\r\n]* -> skip;
BlockComment: '/*' .*? '*/' -> skip;
Whitespace: [ \t\r\n]+ -> skip;

/*
 * Fragments
 */

fragment RParenthesis: ')';
fragment LParenthesis: '(';

