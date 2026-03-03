* [] ValueObject base
* [] Entity base
* [] Aggregate base
* [] UC1 Create new event
  * [] Aggregate VeaEvent
  * [] Value EventId
  * [] S1
  * [] S2
  * [] S3
  * [] S4
* [] UC2 Update event title
  * [] Value EventTitle
  * [] S1
  * [] S2
  * [] F1
  * [] F2
  * [] F3
  * [] F4
  * [] F5
  * [] F6
* [] UC3 Update event description
  * [] Value EventDescription
  * [] S1
  * [] S2
  * [] S3
  * [] F1
  * [] F2
  * [] F3
* [] UC4 Update event start time and end time
  * [] Value EventTimeRange
  * [] S1
  * [] S2
  * [] S3
  * [] S4
  * [] S5
  * [] F1
  * [] F2
  * [] F3
  * [] F4
  * [] F5
  * [] F6
  * [] F7
  * [] F8
  * [] F9
  * [] F10
  * [] F11
* [] UC5 Make event public
  * [] Value EventStatus
  * [] S1
  * [] F1
* [] UC6 Make event private
  * [] S1
  * [] S2
  * [] F1
  * [] F2
* [] UC7 Set event maxNoOfGuests
  * [] Value MaxNoOfGuests
  * [] S1
  * [] S2
  * [] S3
  * [] F1
  * [] F2
  * [] F3
  * [] F4
  * [] F5
* [] UC8 Make event ready
  * [] S1
  * [] F1
  * [] F2
  * [] F3
  * [] F4
* [] UC9 Activate event
  * [] S1
  * [] S2
  * [] S3
  * [] F1
  * [] F2
* [] UC10 Anon registers new guest account
  * [] Aggregate Guest
  * [] Value Name
  * [] Value ViaMail
  * [] Value GuestId
  * [] S1
  * [] F1
  * [] F2
  * [] F3
  * [] F4
  * [] F5
  * [] F6
  * [] F7
* [] UC11 Guest participates in public event
  * [] Entity Participation
  * [] Value ParticipationId
  * [] Value ParticipationStatus
  * [] Value ParticipationSource
  * [] S1
  * [] F1
  * [] F2
  * [] F3
  * [] F4
  * [] F5
* [] UC12 Guest cancels event participation
  * [] S1
  * [] S2
  * [] F1
* [] UC13 Guest is invited to event
  * [] S1
  * [] F1
  * [] F2
  * [] F3
  * [] F4
* [] UC14 Guest accepts invitation
  * [] S1
  * [] F1
  * [] F2
  * [] F3
  * [] F4
  * [] F5
* [] UC15 Guest declines invitation
  * [] S1
  * [] S2
  * [] F1
  * [] F2