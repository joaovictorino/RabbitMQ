'use strict';

const rabbot = require('rabbot');

rabbot.configure({
  connection: {
    name: 'default',
    user: 'guest',
    pass: 'guest',
    host: 'localhost',
    port: 5672,
    vhost: '%2f',
    replyQueue: 'customReplyQueue',
    heartbeat: 5,
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
    rabbot.on('unreachable', () => {
      reconfigurar();
      console.log("retry event");
    });
    console.log('connected!');
  }
);

function logEvery2Seconds(i) {
  setTimeout(() => {
      console.log("mensagem " + i);
      rabbot.publish('ex.1', { type: 'MyMessage', body: 'hello! ' + i })
      .catch(() => {
        console.log("retry catch");
        //rabbot.retry();
        reconfigurar();
      });
      logEvery2Seconds(++i);
  }, 1000)
}

logEvery2Seconds(0);

let i = 0;
setInterval(() => {
    console.log("mensagem");
    rabbot.publish('ex.1', { type: 'MyMessage', body: 'hello!' })
    .catch(() => {
      console.log("retry catch");
      //rabbot.retry();
      reconfigurar();
    });
}, 1000);

function reconfigurar(){
  rabbot.configure({
    connection: {
      name: 'default',
      user: 'guest',
      pass: 'guest',
      host: 'localhost',
      port: 5672,
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
  });
}