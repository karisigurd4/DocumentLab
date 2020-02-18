# Release notes

**1.3.0**

* General
  * Extended language configuration options 
  * Set language in global settings
  * Fuzzy text matching improvements
* TextAnalyzer
  * Contextual data split by language setting
  * Extended date interpretation configurability
  * Added custom text type definitions, made InvoiceNumber a custom definition 
  * Improved fuzzy text comparison
* Query language
  * Added support for logical or || in text type or capture 
  * Query page SubSet (Top, Bottom, Left, Right) 
  * Table analyzer component and language extension 
    * Support in language
    * Support in analyzer
* FluentQuery
  * Added support for logical or operator on text type or capture
  * Added support for subset specifications 
  * Added support for table definitions 
* Bugfixes
  * Fixes for very game breaking bugs in the interpreters
  * Fixed bug where text type was not validated when using parameters 
  * Fixed bug where Json serialization of results could fail when interpretation results were empty

**1.2.0**

* Added Fluent Query API to DocumentLab

**1.1.0 - 1.1.1**

* DocumentLabCL - An executable command line interface program to execute DocumentLab scripts

**1.0.0**

* Initial commit, moved from private repsository to git
* Added script analyzer and json serialization for interpretation results
