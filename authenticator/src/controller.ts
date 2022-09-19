import express from 'express';

class Controller {
    private static readonly Path: string = "/token";

    public router: express.Router;

    public constructor()
    {
        this.router = express.Router();
        this.router.get(Controller.Path, this.authorize);
    }

    private authorize(request: express.Request, response: express.Response)
    {
        request.body;
        response.status(200).send('OK');
    }
}

export default Controller;
