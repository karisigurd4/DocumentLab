import React, { Component } from 'react'

class Card extends Component {

    render()
    {
        const { title, subtitle, content, fullHeight, warning } = this.props

        return (
            <div className="Card">
               <div className="Row">
                   <span>{title}</span>
                   <span>{subtitle}</span>
                   <span>{warning}</span>
                </div>
                <div className={`Row ${fullHeight ? 'FullHeight' : ''}`}>
                    {content && content}
                </div>
            </div>        
        )
    }
}

export default Card;