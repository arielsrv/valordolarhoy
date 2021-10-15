import React from 'react';

export default class Ad extends React.Component {
    componentDidMount() {
        // @ts-ignore
        (window.adsbygoogle = window.adsbygoogle || []).push({});
    }

    render() {
        return (
            <div className='ad'>
                <ins className='adsbygoogle'
                     style={{display: 'block'}}
                     data-ad-client='ca-pub-5089293308725166'
                     data-ad-slot='1331389894'
                     data-ad-format='auto'
                     data-full-width-responsive="true"/>
            </div>
        );
    }
}