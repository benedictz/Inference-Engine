Introduction to Artifical Intelligence
Assignment 2
Group Number	- COS30019_A02_T029
Benedict Zeng	- 6450555
Matthew Harvey	- 7666713

||============================================================================||
||                          I n s t r u c t i o n s                           ||
||============================================================================||

The program must be executed with the following syntax:

	"iengine.exe [file] [method]"

	file	-	The name of the textfile containing the input data.
			Must contain "TELL" followed by a semicolon separated list of clauses, and "ASK" followed by a query clause
			Clauses must use the following syntax:
			~	-	Negation 
			&	-	Conjunction
			||	-	Disjunction
			=>	-	Implication
			<=>	-	Biconditional
			( )	-	Parentheses

	method	-	The search method to be used, which can be one of the following:
			"FC"	-	Forward Chaining
			"BC"	-	Backward Chaining
			"TT"	-	Truth Table / Enumeration
			"RFC"	-	Resolution Forward Chaining



||============================================================================||
||                              G l o s s a r y                               ||
||============================================================================||

ASSERT:
	Determine if new information can be obtained, given a proposition and a set of facts. 

CLAUSE:
	A proposition, which can be either true or false.
	Propositions may be atomic (symbols) or compound (rules).

CONNECTIVE:
	A binary logical operator, defining the relationship between two propositions or logical sentences.
	The connectives used in this project are "Conjunction", "Disjunction", "Implication", and "Biconditional".
	Note that "Negation" is not considered a connective due to it's function as a unary operator.

ENTAIL:
	A binary operator between two sets which may each contain one or more clauses.
	The second set is entailed by the first if it cannot disprove the first set.

EVALUATE:
	Determine if a proposition holds true, given a set of facts.

IDENTITY:
	A string representing the body of a clause, not including any enclosing negation or parentheses.
	It is used to compare clauses which are in direct opposite to one another.

LITERAL:
	A string representing a unique clause, including any enclosing negation and parentheses.

KNOWLEDGE BASE:
	A set of facts in the form of propositional clauses, assumed to be true for a given domain.
	It can also be expressed as the conjunction of each of the constituent clauses.

POLARITY:
	The truth of a clause, which will be true unless the clause is negated.

RULE:
	A compound proposition, a sentence which defines the relationship between two clauses with a logical connective.
	It can evaluate based on the connective, and may assert new information.

SYMBOL:
	An atomic proposition, the smallest unit of propositional logic.
	It can only evaluate itself, based on it's polarity, and can assert no new information.

||============================================================================||
||                              F e a t u r e s                               ||
||============================================================================||

FORWARD CHAINING:

	Forward Chaining is a data driven search method. It works with a set of clauses in horn form, starting with the symbols
	and forwards propogating to find new information until either the query is found or no new information can be obtained.
	
	This is implemented using the Hornform Relational Database (below) which ensures that Forward Chaining runs in linear
	time. This is because the database provides direct O(1) access to the set of clauses which contain some symbol p in
	their premise. Forward Chaining uses a counter for the number of symbols in the premise of each clause (also generated 
	as part of the database) and decrements this count each time a symbol is proven to be true. The search starts with the
	facts contained in the database and then applies these to the clauses, using the database to lookup which clauses need to
	be informed when new information is obtained.

BACKWARD CHAINING:

	Backward Chaining is a goal driven search method. It works backwards from a query, recursively searching for clauses
	which can prove sub-goals, until some path is found which proves the query, or all paths have been exhausted.

	This is implemented again using the Hornform Relational Database (below) to provide O(1) look up time for the
	relationship between clauses, symbols in their premise, and the symbols they prove as a result. Backward Chaining
	starts with the premise in the agenda, and then looks up which clauses could prove that query, and then which symbols
	prove those clauses. It treats each of these symbols as a new subgoal, and continues this search recursively until a
	clause is proven directly using facts from the database.

TRUTH TABLES:

	Truth Table Enumeration is an exhaustive search method which generates all possible models for the set of symbols in
	the domain of the Knowledge Base, checking each model to see if it is entailed by the Knowledge Base. In this program
	the Truth Table search method has been extended to work with general Knowledge Bases, not limited to clauses in horn form.
	This was achieved using the KnowledgeBase and Clauses data types (below). These custom data types provide an object
	oriented interface through which a clause can be Evaluated as either true or false within a given Knowledge Base. This
	was extended to define an "entails" function, which then allowed the Truth Table method to work with any form of Knowledge
	Base and still check whether it entails the models being generated.

	Because it enumerates all possible models for all symbols within a set of clauses, the Truth Table search method has a
	complexity of O(n^2). This performance is significantly worse than the chaining methods, however the trade off is that
	it provides a complete search for all possible clauses without restriction to their form.

RESOLUTION FORWARD CHAINING:

	Resolution Forward Chaining is an attempt to improve the search through a general knowledge base by simplifying the search method, 
	applying forward chaining logic to a general knowledge base. This again uses the KnowledgeBase and Clause class types below, but 
	extends that functionality further with the ability to Assert new information, by assuming a clause is true within a given KnowledgeBase.

	With this method, each clause of the Knowledge Base is then asserted in order, with successful assertions then marking that
	clause as "closed" so that it will not be asserted again. Whenever new information is asserted and added to the KnowledgeBase,
	the search is called recursively, while skipping over any "closed" clauses. This ensures that the order of clauses in the
	KnowledgeBase does not prevent a query from being found, however it does impact the performance.

	The logic for this method is sound, but the search is not currently complete due to the fact that the Asert function for the
	Clause datatypes has not yet been fully defined with all rules of propositional calculus (see below).

HORNFORM RELATIONAL DATABASE:

	The Forward Chaining and Backward Chaining algorithms have been optimised using a relational database of the horn form
	clauses. This involves indexing every clause by the symbols contained in the premise and conclusion, and indexing every
	symbol by the clauses whose premise or conclusion it appears in.
	
	This reduces the search time through the list of clauses from O(n) to O(1), allowing direct access to any information
	needed. Because both Forward and Backward chaining need to perform some search with each element of the agenda, this in
	turn reduces the complexity of both chaining methods from O(n^2) to O(n), where n is proportional to the number of elements
	that get added to the agenda.

	For example, the following KB would generate the tables below
	
	A&B => L;
	A&P => L;
	L&M => P;
	B&L => M;
	P   => Q;

	         ╔═══╦═══╦═══╦═══╦═══╦═══╗
	 SYMBOLS ║ A ║ B ║ L ║ M ║ P ║ Q ║
	╔════════┌───┬───┬───┬───┬───┬───┐
	║ A^B=>L │ T │ T │   │   │   │   │
	╠════════├───┼───┼───┼───┼───┼───┤
	║ A^P=>L │ T │   │   │   │ T │   │
	╠════════├───┼───┼───┼───┼───┼───┤
	║ B^L=>M │   │ T │ T │   │   │   │
	╠════════├───┼───┼───┼───┼───┼───┤
	║ L^M=>P │   │   │ T │ T │   │   │
	╠════════├───┼───┼───┼───┼───┼───┤
	║   P=>Q │   │   │   │   │ T │   │
	╚════════└───┴───┴───┴───┴───┴───┘
	
	         ╔═══╦═══╦═══╦═══╦═══╦═══╗
	 RESULTS ║ A ║ B ║ L ║ M ║ P ║ Q ║
	╔════════┌───┬───┬───┬───┬───┬───┐
	║ A^B=>L │   │   │ T │   │   │   │
	╠════════├───┼───┼───┼───┼───┼───┤
	║ A^P=>L │   │   │ T │   │   │   │
	╠════════├───┼───┼───┼───┼───┼───┤
	║ B^L=>M │   │   │   │ T │   │   │
	╠════════├───┼───┼───┼───┼───┼───┤
	║ L^M=>P │   │   │   │   │ T │   │
	╠════════├───┼───┼───┼───┼───┼───┤
	║   P=>Q │   │   │   │   │   │ T │
	╚════════└───┴───┴───┴───┴───┴───┘

KNOWLEDGEBASE AND CLAUSES:
	
	This program uses an object oriented approach in storing the Knowledge Base. A clause is defined as an abstract "Clause" object
	which provides an interface for the common elements of a proposition (such as the polarity and identity), while specific clauses
	are implemented as either "Rule" objects or "Symbol" objects. Symbols contain a single symbol with a polarity, representing the 
	smallest atomic unit of propositional logic. Rules contain two clauses (antecedent and consequent) connected by a logical
	connective, where each clause may in turn be either a Rule or a Symbol.

	The Knowledge Base is then represented as a collection of these clauses. This collection is stored in a dictionary and indexed by
	the identity of each clause, as this ensures that the Knowledge Base does not contain duplicates or contradictions, and it allows
	for O(1) look up time when trying to determine if a given clause is known in the Knowledge Base.

	Because a Knowledge Base may or may not contain a definitive answer for the truth of any given clause, ternary logic has been used
	to evaluate clauses, implemented with nullable boolean types. This ensures that the Knowledge Base can differentiate between when
	a clause is known to be "False", and when a clause is not known at all.

	This is then implemented in the Evaluate functions for Rules and Symbols, which inherit the method from the abstract Clause object.
	Symbols evaluate themselves for a given KB by checking simply if the KB contains a symbol with same identity, and then comparing
	the two polarities. Rules evaluate themselves by checking first if they exist in the KB as a whole clause, and if not, then by
	evaluating the antecedent and consequent respectively, and performing a bitwise comparison of the results, while retaining the null
	logic.

	In this way, the evaluate function becomes recursive, allowing nested clauses to evaluate themselves until a definitive answer is reached,
	or the answer is proven to be unknown.

	Finally, clauses define the ability to assert new information with an Assert() function. This function assumes that the clause is true
	for a given KB, and then determines if any new information can be learnt. For symbols this yields no results, as they are already the
	smallest unit of logic that can be represented. For rules, this is achieved by evaluating the antecedent and consequent, and making
	returning new information based on the assumption that the rule as a whole must be true.


PROPOSITIONAL CALCULUS:

	The following rules of propositional calculus have been implemented and accounted for in the program. This happens
	during Clause construction, and then during searching. The search method attempts to resolve the Knowledge Base by
	asserting if each clause can determine new information, which in turn is done by evaluating the antecedent ("A") and
	conesequent ("C") within the knowledge base.

	Double Negation
		A double negation is the same as no negation.
		This is implemented by recursively removing negation when parsing the string.

	Commutativity
		The order of the premises is irrelevant for Conjunction, Disjunction, and Biconditional clauses.
		This is implemented in the Evaluate() and Assert() functions which check both sides of each clause.

	De Morgan's Laws
		The negation of a disjunction or conjunction is equal to the conjunction or disjunction of the negation of each clause.
		This is implemented in the Assert() functions for Conjunction and Disjunction rules.

	Biconditional Introduction
		Given two clauses which imply each other, the biconditional of those clauses can be implied.
		This is implemented by the Evaluate() function of a Biconditional rule.
		It will return true if A and C have the same value.

	Biconditional Elimination
		Given a biconditional of two clauses, and either of the clauses, the other clause can be implied.
		This is implemented by the Assert() function of a Biconditional rule.
		It will infer A if C is true.
		It will infer C is A is true.
		It will infer ~A if C is false.
		It will infer ~C is A is false.

	Implication Introduction
		If one fact necessarily follows another, then their implication can be inferred.
		This means that the rule holds true if A is false or C is true.
		This is implemented by the Evaluate() function of an Implication rule.

	Implication Elimination / Modus Ponens
		From an implication clause and its antecedent, the consequent can be inferred.
		This is implemented by the Assert() function of an Implication rule.
		It will infer C if A is true.
	
	Modus Tollens
		Given an implication and the negation of its conclusion, the negation of its premise can be inferred.
		This is implemented by the Assert() function of an Implication rule.
		It will infer ~A if C is false.

	Modus Ponendo Tollens
		Given the negation of a conjunction and one of the premises, the negation of the other premise can be inferred.
		This is implemented by the Assert() function of a negated Conjunction rule.
		It will infer A if C is true.
		It will infer C if A is true.

	Conjunction Introduction
		From a list of facts, their conjunction can be inferred.
		This is implemented by the Evaluate() function of a Conjunction rule.
		It will return true if both A and C are true.

	Conjunction Elimination
		From a conjunction, all of the conjuncts can be inferred.
		This is implemented by the Assert() function of a Conjunction rule.
		It will infer A.
		It will infer C.

	Disjunction Introduction
		From a fact, it's disjunction with anything can be inferred.
		This is implemented by the Evaluate() function of a Disjunction rule.
		It will return true if A or C are true.

		

||============================================================================||
||                                I s s u e s                                 ||
||============================================================================||

CONTRADICTIONS:

	The KnowledgeBase can only contain unique propositional identities. This is done to ensure that
	KnowledgeBases can correctly identify two clauses as being direct opposites, and use this information to
	establish that a clause is proven or disproven by the KnowledgeBase.
	
	This means that it is not possible to store a contradiction, such as "A" and "~A", in the same KnowledgeBase.
	Presently the program does not have a method of resolving these conflicts fairly, and so it will display an error message,
	ignore the second clause, and continue.

	Similarly, it is possible to define KnowledgeBases which will contradict themselves and be unable to yield new information,
	such as ~(A=>B) and B, as the implication will always be true when the consequent is true, so the rule cannot be satisfied
	in this KnowledgeBase.

	In summary, contradictions can occur in the following instances:
		- When passing a set of clauses containing a contradiction into the KnowledgeBase constructor.
		- When adding a new clause which contradicts an existing clause in the KnowledgeBase.
		- When asserting a negated implication, and the antecedent is false, or the consequent is true.

MISSING RESOLUTIONS:
	
	The current implementation of Assert() and Evaluate() makes use of many rules of propositional calculus, however
	not all of the rules have been implemented, due to their complexity and the time constraints of this project.
	Because of this, there are some edge cases where the RFC method will fail to propogate the correct result.

||============================================================================||
||                            T e s t   C a s e s                             ||
||============================================================================||

We utilised a number of custom test cases to cover as much ground as possible throughout development, fixing our code as we ran into
new edge cases. The test provided by the assignment brief covered basic implication '=>' and connective '&', and provided a knowledge
base that allowed the user to successfully assert the query.

RETURNS TRUE:

	The first test we created checked that a basic test that returned true did so properly:

	TELL
	a=>b; b=>c; c=>d; a;
	ASK
	c

	This would check that the simple solution of 'a b c' would be discovered, and additionally that the 'd' wouldnt throw the
	search off. We then extended this test to a more complex form, this time including conjunctions:

	TELL
	a&b=>d; b=>c; c&a=>d; b&d=>e; c&e=>f; d=>b; a; c;
	ASK
	f

RETURNS FALSE & SOLUTION UNREACHABLE:

	The next test was to see that a basic test that returned false did so properly:

	TELL
	a=>b; c=>d; b&d=>e; e=>c; a&d=>f; a&b =>g; a;
	ASK
	f

	This was extended in the next test, which checked if the fact was unreachable. This was tailored specifically for Backwards
	Chaining and the Truth Table:

	TELL
	b=>c; c=>d; a;
	ASK
	d

SOLUTION NONEXISTENT

	We then checked if the solution was not in the Knowledge Base at all:

	TELL
	a=>b; b=>c; c=>e; d=>f; a;
	ASK
	g

INFINITE LOOPS WITH AND WITHOUT SOLUTION

	We tested for infinite loops, to ensure the method doesn't constantly search through the rules:

	TELL
	A=>B; B=>C; C=>A;
	ASK
	C

	As well as loops that had a fact provided, in order to prove that our search method checked facts before depending on rules:

	TELL
	A=>B; B=>C; C=>A; A;
	ASK
	C

MULTIPLE PATH SOLUTIONS

	We tested for multiple solutions. This was to check which path each test case provided back as its result:

	Forward Chaining returns 'A, B, L, M, G, P, Q', due to the fact that Forward Chaining essentially branches out from the facts
	until it finds the query, acting as a brute force method.
	
	Backward Chaining returns 'A, B, L, G, X, Q', which is shorter. This is because Backward Chaining begins from the query, and
	works towards the given facts. This will inherently always be shorter than Forward Chaining by design, as it will not concern
	itself with any other facts, so long as it reaches the first collection of facts that can prove the query, it will disregard
	the rest.

	TELL
	L&M => P; B&L => M; A&B=>L; X=> Q; P => Q; F => X; G => X; L => G; A; B;
	ASK
	Q


MULTIPLE DEPENDENCIES

	We tested for multiple dependencies to ensure that the program wouldnt fail to prove any clauses if it was checked in the
	wrong order:

	This was created specifically because our program previously prematurely returned a negative result.

	TELL
	a&b=>c; b&c=>d; c&d=>e; d&e=>f; a; b;
	ASK
	f

CONTRADICTIONS

	We created this to check for contradicting rules. This would not work on the Forward Chaining or Backward Chaining methods, as
	they only allow Horn Clauses. However, the Truth Table method as well as our custom Resolution Forward Chaining method would
	be able to detect these contradictions:

	However, we have not been able to find a completely fair solution to handle this outcome aside from just discarding new contradictions.

	TELL
	~(a=>b); a
	ASK
	b

CONJUNCTIVE CONSEQUENT

	This test was to see if our program handled conjunctives as the consequent.
	This test is specifically for the Truth Table method and our custom Resolution Forward Chaining method.

	TELL
	a=>b; c=>d; a; c
	ASK
	b&d
	
DISJUNCTIVE CONSEQUENT

	This test was to see if our program handled disjunctions as the consequent.
	This test is specifically for the Truth Table method and our custom Resolution Forward Chaining method.

	TELL
	a=>b; c=>d; a; c
	ASK
	e||d

GENERIC KNOWLEDGE BASE
	
	This test was an implementation of every operation that can be used in a general knowledge base. It utilises biconditional, negation,
	brackets and disjunction to form a complex relationship for the search method to evaluate.

	It works successfully.

	TELL
	(a <=> (c => ~d)) & b & (b => a); ~f || g; c;
	ASK
	~d

||============================================================================||
||                      A c k n o w l e d g e m e n t s                       ||
||============================================================================||

Set 6: Knowledge Representation: The Propositional Calculus - Chapter 7 R&N - 2016 - Kalev Kask
Artificial Intelligence: A Modern Approach - Third Edition - Stuart Russell and Peter Norvig

||============================================================================||
||                        S u m m a r y   R e p o r t                         ||
||============================================================================||

Benedict Zeng

	- Created the Main structure, parsing text file into arguments
	- Tested the KnowledgeBase and basic Clause constructor
	- Created the parser that understands all connective types
	- Implemented Rule Clauses
		- Conjunction
		- Disjunction
		- Implication
		- Biconditional
	- Tested clause functionality
	- Worked on methods:
		- Forward Chaining
		- Backward Chaining
		- Truth Table

Matthew Harvey

	- Created custom types such as:
		- the Clauses to Constants and Rules
		- the underlying KnowledgeBase object type
		- the Node object types
	- Created the parse that understands parentheses and negation
	- Created and worked on methods:
		- Forward Chaining
		- Backward Chaining
		- Truth Table
		- Resolution Forward Chaining
	- Created the custom Horn Form Database

COLLECTIVE

	Throughout the entirety of development, communication was held through online text mediums and voice chat, as well as in person meetings and work sessions. Content was often shared with each other over messaging apps, including screenshots, photos of each other's hand-written work and online links. We also utilised the document shared working service Google Docs to go over more complex issues together. This culminated in shared Truth Table data and manual Forward and Backward Chaining methods. Discord was the primary voice app used to talk during all of these online interactions, which were held almost daily.

	Functions that each member had created was then tested by the other member to ensure quality. Test cases were created and run individually by each member, and then shared with each other so that testing could be done at the same time when unique edge case bugs were discovered. This would then be rectified together in the code.

	In approximately separating the overall workload, Matthew worked on XX% and Benedict worked on XX%.