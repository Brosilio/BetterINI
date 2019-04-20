# Sleepingmat
Sleepingmat is essentially a more advanced version of the most basic .ini file.
It stores settings in dictionaries, lists, and other terrible things.

## Format
Sleepingmat does not support comments inside files. Sorry, nerds.

Key:value pairs can be put inside blocks or left on their own.

If there are duplicate keys, the last

```json
Setting: "value";

blockname
{
    number: 69;
    string: "piss";
    string2: "eat. shit.";
}
```

## Syntax

Sleepingmat is a key:value format.

Keys can be anything that does not start with a number or have non-alphanumeric characters.

Values must be encased in quotes if they're not numbers.
`num: 69;`

vs

`string: "cheers, mate";`

Key:value pairs must always end with a semicolon.

Blocks are created by adding a block name, then an opening brace.

`blockname {}`

Inside a block, you can put as many key/value pairs as you need.

don't even THINK about putting a block inside a block. that is `not allowed`.

## License
BSD 3 santa claus