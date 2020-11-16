# Robert's Full Text Search

This library translates a Google like search query into an SQL Server 
[CONTAINS(..)](https://docs.microsoft.com/en-us/sql/t-sql/queries/contains-transact-sql?view=sql-server-ver15)
query. By default individual words are AND'd, unless an explicit OR is used. 
Quoted words means and exact match. Words prefixed with a minus sign are exclusions.


<dl>
<dt>Exclude words from your search</dt>
<dd>Put - in front of a word you want to leave out. For example, jaguar speed -car</dd>

<dt>Search for an exact match</dt>
<dd>Put a word or phrase inside quotes. For example, "tallest building".</dd>

<dt>Combine searches</dt>
<dd>Put "OR" between each search query. For example, marathon OR race.</dd>

</dl>

#### Example Queries


<dl>
<dt>running</dt>
<dd>
    Return matchs that contain running, and any forms of running like ran, runs, or run.
</dd>

<dt>running leaping</dt>
<dd>
    Return matches that contain both running and leaping, or any forms of 
    of running or leaping.
</dd>

<dt>"running leaping"</dt>
<dd>
    Return matchs that contain the exact phrase running leaping. 
<dd>

<dt>running +leaping</dt>
<dd>
    Return matchs that contain the both running and any form of running
    and the exact word leaping. 
<dd>

<dt>running -leaping</dt>
<dd>
    Return matches that contain running but not leaping, or any forms of 
    of running or leaping.
</dd>

<dt>running OR leaping</dt>
<dd>
    Return matches that either running or leaping or both of them, or any forms of 
    of running or leaping.
</dd>

<dt>run*</dt>
<dd>
    Return matches that contain words that start with exactly run. 
</dd>

</dl>

#### Nonsense queries

Not all queries make sense.

<dl>
<dt>-running</dt>
<dd>
    A query of all exclusionay terms returns an empty string. 
</dd>

<dt>[... stop words ...]</dt>
<dd>
    A stop word is a short common word that is ignored. Words like
    "the", "a", "an", "is" and so forth. A query that is all stop
    words returns an empty string.
</dd>
</dl>


#### Logical Operators

A B C means A and B and C. With matches containing all three ranked higher than matches 
with only 1 or 2 matches.

A B OR C D means A and (B or C) and D. Apparently this is intuitive for more people. It
is confusing and seemingly wrong for people with programming experience.

A B -C means A AND B AND NOT C. 


a b c d e f g h i j

a OR b c d e f g h i j = (a or b) and c and d...

a b OR c d OR e 