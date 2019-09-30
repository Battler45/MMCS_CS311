%{
// Эти объявления добавляются в класс GPPGParser, представляющий собой парсер, генерируемый системой gppg
    public BlockNode root; // Корневой узел синтаксического дерева 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%union { 
			public double dVal; 
			public int iVal; 
			public string sVal; 
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
			public IdentListNode idsVal;
			public VarDefNode varVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END CYCLE ASSIGN SEMICOLON WHILE DO REPEAT UNTIL FOR TO WRITE LEFT_BRACKET RIGHT_BRACKET IF THEN ELSE VAR COMMA PLUS MINUS MULTIPLICATION DIVISION
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident T F
%type <stVal> assign statement cycle while repeat for write if 
%type <blVal> stlist block
%type <idsVal> identList
%type <varVal> var 

%%

progr   : block { root = $1; }
		;

stlist	: statement 
			{ 
				$$ = new BlockNode($1); 
			}
		| stlist SEMICOLON statement 
			{ 
				$1.Add($3); 
				$$ = $1; 
			}
		;

statement: assign { $$ = $1; }
		| block   { $$ = $1; }
		| cycle   { $$ = $1; }
		| for	  { $$ = $1; }
		| while	  { $$ = $1; }
		| repeat  { $$ = $1; }
		| write	  { $$ = $1; }
		| if	  { $$ = $1; }
		| var	  { $$ = $1; }
	;

ident 	: ID { $$ = new IdNode($1); }	
		;

var	    : VAR identList { $$ = new VarDefNode($2); }
		;

identList: ID 
			{ 
				$$ = new IdentListNode(new IdNode($1)); 
			}
		| identList COMMA ID 
			{ 
				$1.Add(new IdNode($3)); 
				$$ = $1; 
			}
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		;

expr	: T  
		| expr PLUS T { $$ = new BinaryNode($1, $3, "PLUS");}
		| expr MINUS T { $$ = new BinaryNode($1, $3, "MINUS");}
		;

T		: F
		| T MULTIPLICATION F { $$ = new BinaryNode($1, $3, "MULTIPLICATION");}
		| T DIVISION F { $$ = new BinaryNode($1, $3, "DIVISION");}
		;

F		: ident  { $$ = $1 as IdNode; }
		| INUM { $$ = new IntNumNode($1); }
		| LEFT_BRACKET expr RIGHT_BRACKET
		;

block	: BEGIN stlist END { $$ = $2; }
		;

cycle	: CYCLE expr statement { $$ = new CycleNode($2, $3); }
		;

while	: WHILE expr DO statement { $$ = new WhileNode($2, $4); }
		;
	
repeat	: REPEAT stlist UNTIL expr { $$ = new RepeatNode($4, $2); }
		;
	
for		: FOR assign TO expr DO statement { $$ = new ForNode($2, $4, $6); }
		;

write	: WRITE LEFT_BRACKET expr RIGHT_BRACKET { $$ = new WriteNode($3); }
		;

if		: IF expr THEN statement ELSE statement { $$ = new IfNode($2, $4, $6); }
		| IF expr THEN statement { $$ = new IfNode($2, $4); }
		;
	
%%

