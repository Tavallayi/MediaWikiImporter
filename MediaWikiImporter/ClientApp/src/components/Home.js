import React, { Component } from 'react';

export class Home extends Component {
    static displayName = Home.name;

    render() {
        return (
            <div>
                <form>
                    <input type='text' id='search' />
                    <input type='submit' id='searchBtn' value='Search' />
                </form>
            </div>
        );
    }
}
