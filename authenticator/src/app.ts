﻿import express from 'express'

const app = express()
const port = 3000

app.get('/', (_, response) => {
    response.send('Hello world!')
})

app.listen(port, () => {
    return console.log(`Express is listening at port ${port}`)
})
