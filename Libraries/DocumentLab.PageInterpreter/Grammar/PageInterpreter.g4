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
	: Any? ( capture | traverse | textType | rightDownSearch )* SemiColon
	;

rightDownSearch
	: RightDown Steps=Numbers
	;

capture
	: propertyName? LBracket Match=textType RBracket
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
	: Text textTypeParameters?
	;

textTypeParameters
	: Parameters
	;

/*
 * Lexer Rules
 */

Any: 'Any';
RightDown: 'RD';

RBracket: ']';
LBracket: '[';
SemiColon: ';';
Colon: ':';
SingleQuote: '\'';

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

