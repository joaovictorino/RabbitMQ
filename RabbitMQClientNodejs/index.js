'use strict';

const rabbot = require('rabbot');

rabbot.handle('MyMessage', (msg) => {
  console.log('received msg', msg.body);
  msg.ack();
});

rabbot.configure({
  connection: {
    name: 'default',
    user: 'guest',
    pass: 'guest',
    host: 'localhost',
    port: 5673,
    vhost: '%2f',
    replyQueue: 'customReplyQueue',
    heartbeat: 15,
    retryLimit: 10
  },
  exchanges: [
    { name: 'ex.1', type: 'fanout', autoDelete: true }
  ],
  queues: [
    { name: 'q.1', autoDelete: true, subscribe: true },
  ],
  bindings: [
    { exchange: 'ex.1', target: 'q.1', keys: [] }
  ]
}).then(() => {
  rabbot.on('unreachable', () => rabbot.retry());
  console.log('connected!');
});