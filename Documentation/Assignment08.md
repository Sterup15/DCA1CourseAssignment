Notes from Troels' presentation
* Copy paste dispatcher pattern from the presentation
* Decorators:
  * Pipeline behavior, single point of entry, add all kinds of different things to the dispatcher, see the slide "Pipeline behaviour"
  * This is where we put stuff that we want ALL command handlers to do, but only put in one place, very SOLID
  * Move UnitOfWork savechanges to this,