import express from 'express';
import Controller from './controller';

const app = express()
const port = 8080

const controller = new Controller();

app.use('/', controller.router);

app.listen(port, () => {
    return console.log(`Express is listening at port ${port}`)
})

